using System.Diagnostics;
using Descope.Maui;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private readonly DescopeClient _descope;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        _descope = IPlatformApplication.Current!.Services.GetRequiredService<DescopeClient>();

        // Keep the dashboard current when the session is refreshed in the background, or cleared
        _descope.SessionManager.SessionChanged += OnSessionChanged;
    }

    /// <summary>
    /// Restores the session saved by a previous launch, so a signed in user goes straight to the dashboard.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            var session = await _descope.SessionManager.LoadSessionAsync();
            if (session != null)
            {
                ShowDashboard(session);
            }
        }
        catch (Exception ex)
        {
            // the saved session couldn't be read, such as after a keychain error, so the user signs in again
            Debug.WriteLine("Session restore error: " + ex);
            ShowLogin();
        }
    }

    /// <summary>
    /// Raised by the session manager, possibly on a background thread, after a login, refresh or logout.
    /// </summary>
    private void OnSessionChanged(object? sender, DescopeSession? session)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (session != null)
            {
                ShowDashboard(session);
            }
            else
            {
                ShowLogin();
            }
        });
    }

    /// <summary>
    /// Runs the Descope flow in the system browser.
    /// </summary>
    async void OnOidcAuthClicked(object s, EventArgs e)
    {
        try
        {
            AuthResultLabel.Text = "Starting login...";
            var session = await _descope.LoginAsync();
            AuthResultLabel.Text = string.Empty;
            ShowDashboard(session);
        }
        catch (DescopeException ex) when (ex.Code == DescopeErrorCode.Cancelled)
        {
            AuthResultLabel.Text = "Login cancelled.";
        }
        catch (Exception ex)
        {
            AuthResultLabel.Text = $"Error: {ex.Message}";
            Debug.WriteLine("Error: " + ex);
        }
    }

    /// <summary>
    /// Gets a new session JWT with the refresh token.
    /// </summary>
    async void OnRefreshClicked(object sender, EventArgs e)
    {
        try
        {
            var session = await _descope.RefreshSessionAsync();
            ShowDashboard(session);
            await DisplayAlertAsync("Refreshed", $"New session expires at {session.SessionToken.ExpiresAt.ToLocalTime():T}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Refresh failed", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Revokes the refresh token, clears the saved session and resets the UI.
    /// </summary>
    async void OnLogOutClicked(object sender, EventArgs e)
    {
        try
        {
            await _descope.LogoutAsync();
        }
        catch (Exception ex)
        {
            // the local session is cleared even when revoking fails
            Debug.WriteLine("Logout error: " + ex);
        }

        ShowLogin();
        await DisplayAlertAsync("Logged Out", "You have been logged out successfully.", "OK");
    }

    /// <summary>
    /// Resets the UI to the login screen.
    /// </summary>
    private void ShowLogin()
    {
        LoginView.IsVisible = true;
        HomeView.IsVisible = false;
        AccessTokenLabel.Text = string.Empty;
        UserInfoLvw.ItemsSource = null;
        InfoLbl.Text = string.Empty;
        AuthResultLabel.Text = string.Empty;
        CopyTokenBtn.Text = "Copy Token";
    }

    /// <summary>
    /// Shows the user, their tenants and roles, and the session JWT.
    /// </summary>
    private void ShowDashboard(DescopeSession session)
    {
        LoginView.IsVisible = false;
        HomeView.IsVisible = true;

        AccessTokenLabel.Text = session.SessionJwt;
        InfoLbl.Text = $"Welcome, {session.User.Name ?? session.User.Email ?? session.User.UserId}!";

        var details = new List<string>
        {
            $"User ID: {session.User.UserId}",
            $"Email: {session.User.Email} (verified: {session.User.IsVerifiedEmail})",
            $"Session expires: {session.SessionToken.ExpiresAt.ToLocalTime():G}",
            $"Project roles: {string.Join(", ", session.Roles())}",
        };
        details.AddRange(session.TenantIds.Select(t => $"Tenant {t} roles: {string.Join(", ", session.Roles(t))}"));
        details.AddRange(session.SessionToken.Claims.Select(c => $"{c.Key}: {c.Value}"));
        UserInfoLvw.ItemsSource = details;
    }

    /// <summary>
    /// Copies the session JWT to the clipboard and temporarily updates the button text.
    /// </summary>
    async void OnCopyTokenClicked(object sender, EventArgs e)
    {
        var jwt = _descope.SessionManager.Session?.SessionJwt;
        if (!string.IsNullOrEmpty(jwt))
        {
            await Clipboard.Default.SetTextAsync(jwt);
            CopyTokenBtn.Text = "Copied!";
            await Task.Delay(2000);
            CopyTokenBtn.Text = "Copy Token";
        }
    }
}
