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

    // Set the model
    public BlogPost? BlogPostModel { get; set; }

    public IActionResult OnGet()
    {
        // Connect to the Contensis delivery API
        // Connection details set in /Program.cs
        var client = ContensisClient.Create();

        var entryId = HttpContext.Request.Query["entryId"];

        string entryVersionStatus = HttpContext.Request.Headers.TryGetValue("x-entry-versionstatus", out var values) ? values.FirstOrDefault() ?? "published" : "published";

        client.DefaultVersionStatus = Enum.Parse<VersionStatus>(entryVersionStatus, true);

        if (!string.IsNullOrEmpty(entryId))
        {
            // Get the entries by the id
            BlogPostModel = client.Entries.Get<BlogPost>(entryId);
        }

        // return a 404 if BlogId is invalid
        if (BlogPostModel == null)
        {
            return NotFound();
        }

        // Set the page title to the blog title
        ViewData["Title"] = BlogPostModel.Title;

        return Page();
    }
}
