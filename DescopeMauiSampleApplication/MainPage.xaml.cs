using DescopeMauiSampleApplication.Descope;
using DescopeMauiSampleApplication.Services;
using Duende.IdentityModel.OidcClient;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace DescopeMauiSampleApplication;

public partial class MainPage : ContentPage
{

    readonly AuthService _auth;
    private AuthenticationResult? _session;
    List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();


    public MainPage(AuthService auth)  // DI provides it
    {
        InitializeComponent();
        _auth = auth;
    }

    async void OnLoginClicked(object s, EventArgs e)
    {
        Debug.WriteLine("OnLoginClicked called");
        Console.WriteLine("OnLoginClicked called");
        _session = await _auth.SignInAsync(this.Window.Handler.PlatformView);
        Debug.WriteLine($"Session is: {_session}");
        if (_session != null) ShowDashboard(_session);
    }

    async void OnLogoutClicked(object s, EventArgs e)
    {
        await _auth.SignOutLocalAsync();
        claims.Clear();
        await _auth.SignOutRemoteAsync(_session.IdToken);
        ShowLogin();
    }

    private void ShowLogin()
    {
        LoginView.IsVisible = true;
        HomeView.IsVisible = false;
    }

    private void ShowDashboard(AuthenticationResult result)
    {
        LoginView.IsVisible = false;
        HomeView.IsVisible = true;

        String user_full_name = result.Account.Username;

        if (result.ClaimsPrincipal != null)
        {
            foreach (var claim in result.ClaimsPrincipal.Claims)
            {
                if (claim.Type == "name")
                    user_full_name = claim.Value;
                claims.Add(claim);
            }
        }

        Debug.WriteLine(claims.Count);

        UserInfoLvw.ItemsSource = claims;
        InfoLbl.Text = $"Welcome, {user_full_name}! Thanks for using Descope!";
    }
}

