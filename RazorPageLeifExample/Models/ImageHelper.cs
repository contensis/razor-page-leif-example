using Zengenti.Contensis.Delivery;

namespace RazorPageLeifExample.Models;

public static class ImageHelper
{
    public static string? GetImageUrl(Image? image)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return string.Format("https://live-{0}-{1}.cloud.contensis.com{2}", DotNetEnv.Env.GetString("PROJECT_API_ID"), DotNetEnv.Env.GetString("ALIAS"), image?.Asset?.Uri);
        }
        return image?.Asset?.Uri;
    }
}