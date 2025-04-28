using Microsoft.AspNetCore.Mvc.RazorPages;
using Zengenti.Contensis.Delivery;

namespace RazorPageLeifExample.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    // Set the model
public IEnumerable<Zengenti.Contensis.Delivery.Node> BlogNodes { get; set; }  // <-- Add this property

    public void OnGet()
    {
        ViewData["Title"] = "Blogs";

        // Connect to the Contensis delivery API
        var client = ContensisClient.Create();

        // Get our blog node and its children
        var nodes = client.Nodes.GetByPath("blog");
        BlogNodes = nodes.Children(); // Assuming Children() is a valid method
    }

}