using DescopeMauiSampleApplication.Descope;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace DescopeMauiSampleApplication;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<MainPage>();

        var descopeClientConfiguration = new Descope.DescopeClientConfiguration()
        {
            DescopeIssuer = "https://api.descope.com/oauth2/v1",
            ProjectId = "P2y0ZxPeRELhp6pJWC40Rgqk3Mtd",
            RedirectUri = "myapp://callback",
            Browser = new WebBrowserAuthenticator(),
        };

        builder.Services.AddSingleton(new DescopeClient(descopeClientConfiguration));

        return builder.Build();
    }
}

