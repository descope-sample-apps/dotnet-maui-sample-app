using System.Diagnostics;
using Duende.IdentityModel.OidcClient;
using DescopeMauiSampleApplication.Descope;
using DescopeMauiSampleApplication.Services;
using Microsoft.Identity.Client;

namespace DescopeMauiSampleApplication;

public partial class MainPage : ContentPage
{

    readonly AuthService _auth;
    private AuthenticationResult? _session;


    public MainPage(AuthService auth)  // DI provides it
    {
        InitializeComponent();
        _auth = auth;
    }

    async void OnLoginClicked(object s, EventArgs e)
    {
        _session = await _auth.SignInAsync(this.Window.Handler.PlatformView);
        if (_session != null) ShowDashboard();
    }

    async void OnLogoutClicked(object s, EventArgs e)
    {
        await _auth.SignOutAsync();
        ShowLogin();
    }

    private void ShowLogin()
    {
        LoginView.IsVisible = true;
        HomeView.IsVisible = false;
    }

    private void ShowDashboard()
    {
        LoginView.IsVisible = false;
        HomeView.IsVisible = true;
    }

}

