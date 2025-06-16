using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;
using Microsoft.Maui.Devices;
using static System.Formats.Asn1.AsnWriter;

namespace DescopeMauiSampleApplication.Services;

public class AuthService
{
    readonly IPublicClientApplication pca;
    readonly string[] scopes = { "openid", "profile", "email", "descope.claims", "descope.custom_claims" };
    private static string ClientId = "P2y0ZxPeRELhp6pJWC40Rgqk3Mtd"; // TODO: Replace with your own Descope project ID;

    public AuthService()
    {
        pca = PublicClientApplicationBuilder
               .Create(ClientId)
               .WithExperimentalFeatures()
               .WithOidcAuthority($"https://api.descope.com/{ClientId}")
               .WithRedirectUri(GetRedirectUri())
               .WithLogging((level, message, pii) => Debug.WriteLine(message),LogLevel.Verbose, enablePiiLogging: true)
               .Build();
    }

   // Debug.WriteLine($"Running on platform: {DeviceInfo.Current.Platform}");

    static string GetRedirectUri() => DeviceInfo.Platform switch
    {
        var p when p == DevicePlatform.WinUI => "http://localhost",
        var p when p == DevicePlatform.Android => $"msal{ClientId}://auth",
     //   var p when p == DevicePlatform.iOS => "msalP2y0ZxPeRELhp6pJWC40Rgqk3Mtd://auth",
       // var p when p == DevicePlatform.MacCatalyst => "http://localhost:3000",
        _ => throw new PlatformNotSupportedException()
    };

    public async Task<AuthenticationResult> SignInAsync(object parentWindow)
    {
        try
        {
            var result = await pca.AcquireTokenInteractive(scopes)
                .WithParentActivityOrWindow(parentWindow).WithUseEmbeddedWebView(false)
                .ExecuteAsync().ConfigureAwait(false);
            Debug.WriteLine("result is: " + result);

            if (result != null)
            {
                var userName = result.Account.Username;
                Debug.WriteLine($"User signed in: {userName}");
                Console.WriteLine($"User signed in: {userName}");
                return result;
            } else
            {
              Debug.WriteLine("Authentication result is null.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
        }
        return null;
    }

    public async Task SignOutLocalAsync()
    {
        var accounts = await pca.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await pca.RemoveAsync(account);
        }
    }

    public async Task SignOutRemoteAsync(string IdToken)
    {
        string logoutUrl = $"https://api.descope.com/oauth2/v1/logout?id_token_hint={IdToken}&post_logout_redirect_uri=";

        await Launcher.Default.OpenAsync(new Uri(logoutUrl));
    }

   
}