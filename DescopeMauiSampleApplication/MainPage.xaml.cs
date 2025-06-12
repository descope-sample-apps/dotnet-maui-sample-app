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


    public MainPage(AuthService auth)  // DI provides it
    {
        InitializeComponent();
        _auth = auth;
    }

    async void OnLoginClicked(object s, EventArgs e)
    {
        _session = await _auth.SignInAsync(this.Window.Handler.PlatformView);
        if (_session != null) ShowDashboard(_session);
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

    private void ShowDashboard(AuthenticationResult result)
    {
        LoginView.IsVisible = false;
        HomeView.IsVisible = true;

        String name = "";

        List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();

        if (result.ClaimsPrincipal != null)
        {
            foreach (var claim in result.ClaimsPrincipal.Claims)
            {
                if (claim.Type == "name")
                    name = claim.Value;
                claims.Add(claim);
            }
        }

        UserInfoLvw.ItemsSource = claims;
        InfoLbl.Text = $"Welcome, {name}! Thanks for using Descope!";

        //var claims2 = DecodeJwt(result.IdToken);

        //UserInfoLvw.ClearLogicalChildren();

        //foreach (var kvp in claims2)
        //{
        //    UserInfoLvw.AddLogicalChild(new Label
        //    {
        //        Text = $"{kvp.Key}: {kvp.Value}",
        //        FontSize = 16,
        //        //Margin = new Thickness(5)
        //    });
        //}



    }

    private Dictionary<string, object>? DecodeJwt(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3)
                return null;

            var payload = parts[1];
            var json = DecodeBase64(payload);
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            return null;
        }
    }

    private string DecodeBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
            case 0: break;
            default: base64 += "="; break;
        }

        var bytes = Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

}

