using Android.App;
using Android.Content;
using Android.Content.PM;
using Descope.Maui;

namespace MauiApp1.Platforms.Android
{
    // Receives the redirect back from the browser. DataScheme must match the scheme of DescopeOptions.RedirectUri
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
                  Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
                  DataScheme = "descopemaui")]
    public class WebAuthenticatorCallbackActivity : DescopeCallbackActivity
    {
    }
}
