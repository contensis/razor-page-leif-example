# Using Contensis with .NET Razor Pages

This step by step guide will take you through getting your entries from Contensis and displaying them using the delivery API and a simple Node.js app.

## Requirements

* [Git](https://git-scm.com/downloads)
* Command line interface knowledge

### Visual Studio

* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/#download) with the ASP.NET and web development workload.

### VS Code

* [Visual Studio Code](https://code.visualstudio.com/download)
* [C# for Visual Studio Code (latest version)](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

## Using the demo project

This app will pull in data from the Leif project in Contensis. The Razor Pages demo is used so you can see how a simple app can use the delivery API.

To get started:

* Clone the Contensis Razor Pages project

```shell
git clone https://gitlab.zengenti.com/ps-projects/leif-example-sites/razor-page-leif-example.git
```

* Change directory to RazorPageLeifExample

```shell
cd RazorPageLeifExample
```

* Run with hot reloading

```shell
dotnet watch
```

The Razor Pages example will open up in your browser.

## How it works

### Include the Contensis delivery API helper

The Contensis delivery API helper contains classes to perform the repetitive tasks of retrieving content from the API.

Include an instance of ```Zengenti.Contensis.Delivery```:

```c#
// Program.cs
using Zengenti.Contensis.Delivery;
```

### Set the connection details globally

```c#
// Program.cs
ContensisClient.Configure(new ContensisClientConfiguration(
    rootUrl: "<root-url>",
    projectApiId: "<projectApiId>",
    clientId: "clientId",
    sharedSecret: "<sharedSecret>"
));
```

### Create a class for each Content Type

E.g. the Blog Content Type:

```c#
// Models/BlogPost.cs
using Zengenti.Contensis.Delivery;

namespace RazorPageLeifExample.Models {
    public class BlogPost: EntryModel { // EntryModel gives us access to the Sys object for ID
        public string Title { get; set; } = null!; // Null forgiving - Title can't be null
        public string? LeadParagraph { get; set; }
        public Image? ThumbnailImage { get; set; }
        public Person? Author => Resolve<Person>("author"); // Resolve linked entry so fields are available
        public Category? Category => Resolve<Category>("category"); // Resolve linked entry so fields are available
        public ComposedField? PostBody { get; set; }
    }
}
```

### Get a single blog entry by its id

#### Connect to the Contensis Delivery API

```c#
// Pages/Blog.cshtml.cs
var client = ContensisClient.Create();
```

Pass this class to `client.Entries.Get` to return a strongly typed `BlogPost`.

```c#
// Pages/Blog.cshtml.cs
// Set the model
public BlogPost? BlogPostModel { get; set; }

public void OnGet()
{
    // Connect to the Contensis delivery API
    // Connection details set in /Program.cs
    var client = ContensisClient.Create();

    // Get the id from the querystring
    string BlogId = HttpContext.Request.Query["id"];

    // Get the entries by the id
    BlogPostModel = client.Entries.Get<BlogPost>(BlogId);
}
```

### Use the model in the view

```html
<!-- Pages/Blog.cshtml -->
<div class="blog-hero">
  <h1 class="blog-hero__title">
    @Model.BlogPostModel.Title
  </h1>
  @if(Model.BlogPostModel.ThumbnailImage != null) {
    <img class="blog-hero__img" src="@("http://live.leif.contensis.cloud" + Model.BlogPostModel.ThumbnailImage.Asset.Uri)" alt="@Model.BlogPostModel.ThumbnailImage.AltText"/>
  }
</div>
```

### Get a list of blogs

More information on search queries can be found here: [https://www.contensis.com/help-and-docs/apis/delivery-dotnet/search/query-operators](https://www.contensis.com/help-and-docs/apis/delivery-dotnet/search/query-operators)

```c# 
// Pages/Index.cshtml.cs
// Set the model
  public PagedList<BlogPost>? BlogsPayload { get; set; }
  public void OnGet()
  {
      ViewData["Title"] = "Blogs";

      // Connect to the Contensis delivery API
      // Connection details set in /Program.cs
      var client = ContensisClient.Create();

      // Query the api for entries with a content type of "blogPost"
      // Get the latest versions even if not yet published
      var blogsQuery = new Query(
          Op.EqualTo("sys.contentTypeId", "blogPost"),
          Op.EqualTo("sys.versionStatus", "latest")
      );

      // Get a list of entries matching the blogsQuery
      BlogsPayload = client.Entries.Search<BlogPost>(blogsQuery);
  }
```

### Use the model in the view

```html
<!-- Pages/Index.cshtml -->
@if ((Model.BlogsPayload != null) && (Model.BlogsPayload.TotalCount > 0)) {
    <ul class="blogs">
        @foreach (var blogItem in Model.BlogsPayload.Items) {
            <li class="blog-card">
                <a href="@("/blog?id=" + blogItem.Sys.Id)">
                    <h2 class="blog-card__title mobile">@blogItem.Title</h2>
                    @if (blogItem.ThumbnailImage != null) {
                        <img class="blog-card__img" src="@("http://live.leif.contensis.cloud" + blogItem.ThumbnailImage.Asset.Uri)" alt="@blogItem.ThumbnailImage.AltText" />
                    }
                    <div class="related-blog__content">
                    <h2 class="blog-card__title desktop">@blogItem.Title</h2>
                    <!-- Truncate text as it can sometimes be too long -->
                    @if (blogItem.LeadParagraph != null) {
                        <p class="blog-card__text">@blogItem.LeadParagraph.Substring(0, Math.Min(blogItem.LeadParagraph.Length, 124))&hellip;</p>
                    }
                    @if (blogItem.Category != null) {
                        <span class="category">@blogItem.Category.Name</span>
                    }
                    </div>
                </a>
            </li>
        }
    </ul>
} else {
    <p>No blogs found</p>
}
```