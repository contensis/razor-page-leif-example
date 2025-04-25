using Zengenti.Contensis.Delivery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddRazorPagesOptions(
        (options) =>
        {
            // Override root to always render blog post
            options.Conventions.AddPageRoute("/Home/Index", "/");
            options.Conventions.AddPageRoute("/Blog/BlogPost", "/blogs/{*slug}");
            options.Conventions.Add(new GlobalHeaderPageApplicationModelConvention());
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); NO SUPPORT FOR .NET 9,0 
app.UseStaticFiles();
app.UseRouting();
// app.UseHttpLogging(); NO SUPPORT FOR .NET 9,0 
app.UseStatusCodePagesWithReExecute("/Error");
app.MapRazorPages();

DotNetEnv.Env.TraversePath().Load();

ContensisClient.Configure(
    new ContensisClientConfiguration(
        rootUrl: string.Format("https://api-{0}.cloud.contensis.com", DotNetEnv.Env.GetString("ALIAS")),
        projectApiId: DotNetEnv.Env.GetString("PROJECT_API_ID"),
        clientId: DotNetEnv.Env.GetString("CONTENSIS_CLIENT_ID"),
        sharedSecret: DotNetEnv.Env.GetString("CONTENSIS_CLIENT_SECRET")
    )
);

app.Run();
