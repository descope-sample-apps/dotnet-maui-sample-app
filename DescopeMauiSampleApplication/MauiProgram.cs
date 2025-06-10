//using DescopeMauiSampleApplication.Descope;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

using DescopeMauiSampleApplication.Services;

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

        //builder.Services.AddSingleton<MainPage>();

        var descopeClientConfiguration = new Descope.DescopeClientConfiguration()
        {
            DescopeIssuer = "https://api.descope.com/oauth2/v1",
            //ProjectId = "<YOUR_PROJECT_ID>", // TODO: Replace with your own Descope project ID
            ProjectId = "P2y0ZxPeRELhp6pJWC40Rgqk3Mtd",
            RedirectUri = "myapp://callback",
            //Browser = new WebBrowserAuthenticator(),
        };

        //builder.Services.AddSingleton(new DescopeClient(descopeClientConfiguration));

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}

