using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private string? _accessToken;
    // TODO: Replace with your Descope project ID and redirect URI
    private string clientId = "<DESCOPE_PROJECT_ID>";
    private string redirectUri = "<REDIRECT_URI>";
    private string? acc;
    private string? id;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is about to appear. Handles any initial deep-link display logic.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // This is from the previous deep-linking task.
        // You can remove it if you no longer need to display the initial deep link.
        if (!string.IsNullOrEmpty(App.DeepLinkUrl))
        {
            // DeepLinkUrlLabel.Text = $"App opened with: {App.DeepLinkUrl}";
            Console.WriteLine($"Deep link opened: {App.DeepLinkUrl}");
            App.DeepLinkUrl = null;
        }
    }

    /// <summary>
    /// Starts the OIDC authentication flow when the associated UI element is clicked.
    /// </summary>
    async void OnOidcAuthClicked(object s, EventArgs e)
    {
        try
        {
            AuthResultLabel.Text = "Starting login...";
            // Step 1: Generate an auth url
            var (codeChallenge, codeVerifier) = GeneratePkce();
            var state = Guid.NewGuid().ToString();

            var authorizationEndpoint = "https://api.descope.com/oauth2/v1/authorize";

            var authUrl = $"{authorizationEndpoint}?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                          $"&scope=openid%20profile%20email&state={state}&code_challenge_method=S256&code_challenge={codeChallenge}";

            // Step 2: Redirect to Descope for Authentication
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new WebAuthenticatorOptions()
                {
                    Url = new Uri(authUrl),
                    CallbackUrl = new Uri(redirectUri),
                    PrefersEphemeralWebBrowserSession = true
                });

            AuthResultLabel.Text = "Callback received, exchanging code...";
            string? code = authResult?.Properties?.TryGetValue("code", out var c) == true ? c : null;
            if (string.IsNullOrEmpty(code))
            {
                AuthResultLabel.Text = "Authentication failed: No auth code returned.";
                return;
            }

            // Step 3: Exchange Authorization Code for Access Token
            var tokenEndpoint = "https://api.descope.com/oauth2/v1/token";
            var token = await ExchangeCodeForTokenAsync(tokenEndpoint, clientId, redirectUri, code, codeVerifier);

            acc = token?.Split(',')[0];
            id = token?.Split(',')[1];

            if (string.IsNullOrEmpty(acc))
            {
                AuthResultLabel.Text = "Authentication failed: Could not get token.";
                return;
            }

            ShowDashboard(acc);
        }
        catch (Exception ex)
        {
            AuthResultLabel.Text = $"Error: {ex.Message}";
            Debug.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Handles the logout process: resets the UI, logs out remotely from Descope, and clears local tokens.
    /// </summary>
    async void OnLogOutClicked(object sender, EventArgs e)
    {
        // Reset the UI
        LoginView.IsVisible = true;
        HomeView.IsVisible = false;
        AccessTokenLabel.Text = string.Empty;
        UserInfoLvw.ItemsSource = null;
        InfoLbl.Text = string.Empty;
        AuthResultLabel.Text = string.Empty;
        CopyTokenBtn.Text = "Copy Token";

        // Remotely logout from Descope
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(_accessToken).ToString();
        string logoutUrl = $"https://api.descope.com/oauth2/v1/logout?id_token_hint={id}&post_logout_redirect_uri={redirectUri}";
        await Launcher.Default.OpenAsync(new Uri(logoutUrl));

        // Locally logout and clear access token
        _accessToken = null;
        await DisplayAlert("Logged Out", "You have been logged out successfully.", "OK");
    }

    /// <summary>
    /// Exchanges the authorization code for access and ID tokens at the specified token endpoint.
    /// </summary>
    /// <param name="tokenEndpoint">The token endpoint URL.</param>
    /// <param name="clientId">The Descope project (client) ID.</param>
    /// <param name="redirectUri">The redirect URI registered with the OIDC provider.</param>
    /// <param name="code">The authorization code returned from the authentication step.</param>
    /// <param name="codeVerifier">The PKCE code verifier generated earlier.</param>
    /// <returns>A comma-separated string containing the access token and ID token, or null if the exchange fails.</returns>
    private static async Task<string?> ExchangeCodeForTokenAsync(string tokenEndpoint, string clientId, string redirectUri, string code, string codeVerifier)
    {
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "code_verifier", codeVerifier }
        };

        using var http = new HttpClient();
        using var content = new FormUrlEncodedContent(requestData);
        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = content };

        using var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Token exchange failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        string? idToken = "";
        string? access = "";

        idToken = doc.RootElement.TryGetProperty("id_token", out var idTokenElement)
            ? idTokenElement.GetString()
            : null;
        access = doc.RootElement.TryGetProperty("access_token", out var accessTokenElement)
            ? accessTokenElement.GetString()
            : null;

        return access + "," + idToken;
    }

    /// <summary>
    /// Displays the dashboard by parsing the JWT access token, storing it, and binding user claims to the UI.
    /// </summary>
    /// <param name="accessToken">The raw JWT access token.</param>
    private void ShowDashboard(string accessToken)
    {
        // Switch views
        LoginView.IsVisible = false;
        HomeView.IsVisible = true;

        // Store and Display the raw token
        _accessToken = accessToken;
        AccessTokenLabel.Text = accessToken;

        // Parse the JWT
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var claims = jwtToken.Claims.ToList();

        // Bind UI
        UserInfoLvw.ItemsSource = claims;
        InfoLbl.Text = $"Welcome, {jwtToken.Subject}!";
    }

    /// <summary>
    /// Copies the stored access token to the clipboard and temporarily updates the button text.
    /// </summary>
    async void OnCopyTokenClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_accessToken))
        {
            await Clipboard.Default.SetTextAsync(_accessToken);
            CopyTokenBtn.Text = "Copied!";
            await Task.Delay(2000);
            CopyTokenBtn.Text = "Copy Token";
        }
    }

    /// <summary>
    /// Generates a PKCE code challenge and verifier pair.
    /// </summary>
    /// <param name="size">The byte length of the random verifier. Defaults to 32 bytes.</param>
    /// <returns>
    /// A tuple containing:
    /// <c>code_challenge</c> – the Base64 URL-encoded SHA256 hash of the verifier,  
    /// <c>verifier</c> – the random Base64 URL-encoded string.
    /// </returns>
    private static (string code_challenge, string verifier) GeneratePkce(int size = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[size];
        rng.GetBytes(randomBytes);
        var verifier = Base64UrlEncode(randomBytes);

        var buffer = Encoding.UTF8.GetBytes(verifier);
        var hash = SHA256.Create().ComputeHash(buffer);
        var challenge = Base64UrlEncode(hash);

        return (challenge, verifier);
    }

    /// <summary>
    /// Encodes the given byte array into a Base64 URL-safe string by replacing '+' and '/' characters and trimming padding.
    /// </summary>
    /// <param name="data">The byte array to encode.</param>
    /// <returns>A Base64 URL-encoded string suitable for use in URLs.</returns>
    private static string Base64UrlEncode(byte[] data) =>
        Convert.ToBase64String(data)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
}