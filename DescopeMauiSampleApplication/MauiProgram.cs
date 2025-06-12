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

        
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AuthServer>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}

