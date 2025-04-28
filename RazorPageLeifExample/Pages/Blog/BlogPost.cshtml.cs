using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zengenti.Contensis.Delivery;
using RazorPageLeifExample.Models;

namespace RazorPageLeifExample.Pages;

public class BlogModel : PageModel
{
    private readonly ILogger<BlogModel> _logger;

    public BlogModel(ILogger<BlogModel> logger)
    {
        _logger = logger;
    }

    // Model to hold the blog post
    public BlogPost? BlogPostModel { get; set; }
    public string? BlogNodeId { get; set; }
    public string? BlogEntryId { get; set; }

    // Accept the 'slug' as a route parameter
    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; }  // This will bind to the 'slug' in the URL

    public IActionResult OnGet()
    {
        var client = ContensisClient.Create();

        // Using the slug directly from the route parameter
        var node = client.Nodes.GetByPath(Slug);
        if (node == null)
        {
            return NotFound();
        }

        var entryId = node.EntryId.ToString();

        if (!string.IsNullOrEmpty(entryId))
        {
            BlogPostModel = client.Entries.Get<BlogPost>(entryId);

            if (BlogPostModel == null)
                return NotFound();

            ViewData["Title"] = BlogPostModel.Title;
            return Page();
        }

        return NotFound();
    }
}
