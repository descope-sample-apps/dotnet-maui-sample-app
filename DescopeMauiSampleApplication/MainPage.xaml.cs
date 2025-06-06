using System.Diagnostics;
using Duende.IdentityModel.OidcClient;
using DescopeMauiSampleApplication.Descope;

namespace DescopeMauiSampleApplication;

public partial class MainPage : ContentPage
{
    int count = 0;
    private DescopeClient _descopeClient;
    private LoginResult _authenticationData;

    public MainPage(DescopeClient descopeClient)
    {
        InitializeComponent();
        _descopeClient = descopeClient;
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    public async void OnLoginClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Login button clicked");
        var loginResult = await _descopeClient.LoginAsync();

        if (loginResult.IsError)
        {
            await DisplayAlert("OIDC error", loginResult.Error ?? "unknown", "OK");
            return;
        }

        if (!loginResult.IsError)
        {
            _authenticationData = loginResult;
            LoginView.IsVisible = false;
            HomeView.IsVisible = true;

            UserInfoLvw.ItemsSource = loginResult.User.Claims;
            HelloLbl.Text = $"Hello, {loginResult.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value}";
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    public async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await _descopeClient.LogoutAsync(_authenticationData.IdentityToken);

        if (!logoutResult.IsError)
        {
            _authenticationData = null;
            LoginView.IsVisible = true;
            HomeView.IsVisible = false;
        }
        else
        {
            await DisplayAlert("Error", logoutResult.ErrorDescription, "OK");
        }
    }
}

