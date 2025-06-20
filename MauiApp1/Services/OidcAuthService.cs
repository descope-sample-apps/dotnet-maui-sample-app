using System.Text.Json;
using Microsoft.Maui.Authentication;

namespace MauiApp1.Services
{
    public class OidcAuthService
    {
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _authorizationEndpoint;
        private readonly string _tokenEndpoint;
        private string? _codeVerifier;

        public OidcAuthService(string clientId, string redirectUri, string authorizationEndpoint, string tokenEndpoint)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            _authorizationEndpoint = authorizationEndpoint;
            _tokenEndpoint = tokenEndpoint;
        }

        public async Task<OidcTokenResponse?> AuthenticateAsync(string? scope = null)
        {
            try
            {
                // Generate PKCE code challenge and verifier
                var (codeChallenge, codeVerifier) = Pkce.Generate();
                _codeVerifier = codeVerifier;

                // Build authorization URL
                var authUrl = BuildAuthorizationUrl(codeChallenge, scope);

                // Use WebAuthenticator to handle the OAuth flow
                var result = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(_redirectUri));

                // Extract authorization code from the result
                if (result?.Properties?.TryGetValue("code", out var authCode) == true)
                {
                    // Exchange authorization code for tokens
                    return await ExchangeCodeForTokensAsync(authCode);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OIDC authentication failed: {ex.Message}");
                return null;
            }
        }

        private string BuildAuthorizationUrl(string codeChallenge, string? scope)
        {
            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["response_type"] = "code",
                ["redirect_uri"] = _redirectUri,
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256",
                ["state"] = Guid.NewGuid().ToString()
            };

            if (!string.IsNullOrEmpty(scope))
            {
                parameters["scope"] = scope;
            }

            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            return $"{_authorizationEndpoint}?{queryString}";
        }

        private async Task<OidcTokenResponse?> ExchangeCodeForTokensAsync(string authorizationCode)
        {
            try
            {
                using var httpClient = new HttpClient();
                
                var tokenRequest = new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["client_id"] = _clientId,
                    ["code"] = authorizationCode,
                    ["redirect_uri"] = _redirectUri,
                    ["code_verifier"] = _codeVerifier ?? string.Empty
                };

                var content = new FormUrlEncodedContent(tokenRequest);
                var response = await httpClient.PostAsync(_tokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OidcTokenResponse>(jsonResponse);
                }

                Console.WriteLine($"Token exchange failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token exchange error: {ex.Message}");
                return null;
            }
        }
    }

    public class OidcTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public string? IdToken { get; set; }
        public string? Scope { get; set; }
    }
} 