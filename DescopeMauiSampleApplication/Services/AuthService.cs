using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;
using Microsoft.Maui.Devices;
using static System.Formats.Asn1.AsnWriter;

namespace DescopeMauiSampleApplication.Services;

public class AuthService
{
    readonly IPublicClientApplication pca;
    readonly string[] scopes = { "openid", "profile", "email" };
    private string ClientId = "P2y0ZxPeRELhp6pJWC40Rgqk3Mtd"; // TODO: Replace with your own Descope project ID;

    public AuthService()
    {
        pca = PublicClientApplicationBuilder
               .Create(ClientId)
               .WithExperimentalFeatures()
               .WithOidcAuthority("https://api.descope.com/" + ClientId)
               //.WithOidcAuthority("https://api.descope.com/oauth2/v1")
               .WithRedirectUri(GetRedirectUri())
               .WithLogging((level, message, pii) => Debug.WriteLine(message),LogLevel.Verbose, enablePiiLogging: true)
               //.WithCustomWebUi(new WebBrowserAuthentication())
               .Build();
    }

    Debug.WriteLine($"Running on platform: {DeviceInfo.Current.Platform}");

    static string GetRedirectUri() => DeviceInfo.Platform switch
    {
        var p when p == DevicePlatform.WinUI => "http://localhost",
        var p when p == DevicePlatform.Android => "msalP2y0ZxPeRELhp6pJWC40Rgqk3Mtd://auth",
        var p when p == DevicePlatform.iOS => "msalP2y0ZxPeRELhp6pJWC40Rgqk3Mtd://auth",
        var p when p == DevicePlatform.MacCatalyst => "http://localhost:3000",
        _ => throw new PlatformNotSupportedException()
    };

    public async Task<AuthenticationResult> SignInAsync(object parentWindow)
    {
        try
        {
            var result = await pca.AcquireTokenInteractive(scopes)
                .WithParentActivityOrWindow(parentWindow) //.WithCustomWebUi(new WebBrowserAuthentication())
                //.WithUseEmbeddedWebView(true)
                .ExecuteAsync().ConfigureAwait(false);
            if (result != null)
            {
                var userName = result.Account.Username;
                Console.WriteLine($"User signed in: {userName}");
                return result;
            }
        }
        catch (MsalException ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
        }
        return null;
    }

    public async Task SignOutAsync()
    {
        var accounts = await pca.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await pca.RemoveAsync(account);
        }
    }

   
}