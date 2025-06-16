using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

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
//            .ConfigureLifecycleEvents(lifecycle =>
//            {
//#if ANDROID
//                lifecycle.AddAndroid(android =>
//                {
//                    android.OnCreate((activity, bundle) =>
//                    {
//                        var action = activity.Intent?.Action;
//                        var data = activity.Intent?.Data?.ToString();

//                        if (action == Android.Content.Intent.ActionView && data is not null)
//                        {
//                            Task.Run(() => HandleAppLink(data));
//                        }
//                    });
//                });
//#endif
//            });
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }

    static void HandleAppLink(string url)
    {
        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            App.Current?.SendOnAppLinkRequestReceived(uri);
    }
}

