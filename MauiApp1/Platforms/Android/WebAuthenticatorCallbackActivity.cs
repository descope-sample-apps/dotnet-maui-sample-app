using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace MauiApp1.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
                  Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
                  DataScheme = "<YOUR_DATA_SCHEME>")] //TODO: Replace with your actual data scheme, e.g., "myapp"
    public class WebAuthenticatorCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
    {
    }
} 