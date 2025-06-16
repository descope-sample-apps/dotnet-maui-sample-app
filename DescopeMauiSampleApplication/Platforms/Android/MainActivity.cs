using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace DescopeMauiSampleApplication;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

[IntentFilter(
        new string[] {Intent.ActionView},
        Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "msalp2y0zxperelhp6pjwc40rgqk3mtd",  // your lowercased scheme
        DataHost = "auth", //domain from which URIs will originate 
        AutoVerify = true
    )]

public class MainActivity : MauiAppCompatActivity
{


    //protected override void OnResume()
    //{
    //    base.OnResume();
    //    Platform.OnResume(this);
    //}


    ///// <summary>
    ///// Called when Android deep-links back into your already running activity.
    ///// We pass the intent to the WebAuthenticator so your login continuation fires.
    ///// </summary>
    //protected override void OnNewIntent(Intent intent)
    //{
    //    base.OnNewIntent(intent);

    //    Platform.OnNewIntent(intent);

    //    Android.Util.Log.Debug("AUTH", $"Intent received: {intent?.Data}");
    //}

    //var action = intent?.Action;
    //var data = intent?.Data?.ToString();

    //if (Intent.ActionView.Equals(action) && data is not null)
    //{
    //    // Handle the deep link URL here
    //    // For example, you can log it or process it as needed
    //    System.Diagnostics.Debug.WriteLine($"Received deep link: {data}");
    //    // Optionally, you can send the URI to your app's main logic

    //    Task.Run(() => HandleAppLink(data));


    //    //if (Uri.TryCreate(data, UriKind.RelativeOrAbsolute, out var uri))
    //    //{
    //    //    App.Current?.SendOnAppLinkRequestReceived(uri);
    //    //}
    //}


    // Pass the intent to MSAL (or your OIDC client)
    // Microsoft.Identity.Client.AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(intent);
}

    //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    //{
    //    System.Diagnostics.Debug.WriteLine($"OnActivityResult called: requestCode={requestCode}, resultCode={resultCode}, data={data}");
    //    //base.OnActivityResult(requestCode, resultCode, data);
    //    //Microsoft.Identity.Client.AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    //}

//static void HandleAppLink(string url)
//{
//    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
//        App.Current?.SendOnAppLinkRequestReceived(uri);
//}




