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

	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// This is from the previous deep-linking task.
		// You can remove it if you no longer need to display the initial deep link.
		if (!string.IsNullOrEmpty(App.DeepLinkUrl))
		{
			DeepLinkUrlLabel.Text = $"App opened with: {App.DeepLinkUrl}";
			Console.WriteLine($"Deep link opened: {App.DeepLinkUrl}");
			App.DeepLinkUrl = null;
		}
	}

	async void OnOidcAuthClicked(object s, EventArgs e)
	{
		try
		{
			AuthResultLabel.Text = "Starting login...";
			// Step 1: Generate an auth url
			var (codeChallenge, codeVerifier) = GeneratePkce();
			var state = Guid.NewGuid().ToString();
			
			// NOTE: Use the new Client ID and Redirect URI from your example
			var clientId = "P2y0ZxPeRELhp6pJWC40Rgqk3Mtd";
			var redirectUri = "msalp2y0zxperelhp6pjwc40rgqk3mtd://auth";
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

			if (string.IsNullOrEmpty(token))
			{
				AuthResultLabel.Text = "Authentication failed: Could not get token.";
				return;
			}

			ShowDashboard(token);
		}
		catch (Exception ex)
		{
			AuthResultLabel.Text = $"Error: {ex.Message}";
			Debug.WriteLine("Error: " + ex.Message);
		}
	}

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
		if(!response.IsSuccessStatusCode)
		{
			Debug.WriteLine($"Token exchange failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
			return null;
		}

		var json = await response.Content.ReadAsStringAsync();
		using var doc = JsonDocument.Parse(json);

		return doc.RootElement.TryGetProperty("access_token", out var accessTokenElement)
			? accessTokenElement.GetString()
			: null;
	}

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

		// Find a display name
		string userFullName =
				claims.FirstOrDefault(c => c.Type == "name")?.Value ??
				jwtToken.Subject;

		// Bind UI
		UserInfoLvw.ItemsSource = claims;
		InfoLbl.Text = $"Welcome, {userFullName}!";
	}

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

	// PKCE Helper methods
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

	private static string Base64UrlEncode(byte[] data) =>
		Convert.ToBase64String(data)
			.Replace("+", "-")
			.Replace("/", "_")
			.TrimEnd('=');
}
