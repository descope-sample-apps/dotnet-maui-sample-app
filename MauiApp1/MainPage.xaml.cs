namespace MauiApp1;

using MauiApp1.Services;

public partial class MainPage : ContentPage
{
	int count = 0;
	private OidcAuthService? _authService;

	public MainPage()
	{
		InitializeComponent();
		InitializeAuthService();
	}

	private void InitializeAuthService()
	{
		// Replace these with your actual OIDC provider configuration
		var clientId = "P2lNntS8Qp7U95NiurYFQhK1nDTR";
		var redirectUri = "mauiapp://oauth/callback";
		var authorizationEndpoint = "https://api.descope.com/oauth2/v1/authorize";
		var tokenEndpoint = "https://api.descope.com/oauth2/v1/token";

		_authService = new OidcAuthService(clientId, redirectUri, authorizationEndpoint, tokenEndpoint);
	}

	private async void OnOidcAuthClicked(object? sender, EventArgs e)
	{
		if (_authService == null)
		{
			AuthResultLabel.Text = "Authentication service not initialized";
			return;
		}

		try
		{
			OidcAuthBtn.IsEnabled = false;
			AuthResultLabel.Text = "Authenticating...";

			var tokenResponse = await _authService.AuthenticateAsync("openid profile email");

			if (tokenResponse != null)
			{
				AuthResultLabel.Text = $"Authentication successful!\nAccess Token: {tokenResponse.AccessToken?[..20]}...\nToken Type: {tokenResponse.TokenType}";
				Console.WriteLine($"OIDC Authentication successful - Access Token: {tokenResponse.AccessToken}");
			}
			else
			{
				AuthResultLabel.Text = "Authentication failed";
				Console.WriteLine("OIDC Authentication failed");
			}
		}
		catch (Exception ex)
		{
			AuthResultLabel.Text = $"Authentication error: {ex.Message}";
			Console.WriteLine($"OIDC Authentication error: {ex.Message}");
		}
		finally
		{
			OidcAuthBtn.IsEnabled = true;
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (!string.IsNullOrEmpty(App.DeepLinkUrl))
		{
			DeepLinkUrlLabel.Text = App.DeepLinkUrl;
			Console.WriteLine($"Deep link opened: {App.DeepLinkUrl}");
			// Reset the deep link URL so it's not processed again
			App.DeepLinkUrl = null;
		}
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
