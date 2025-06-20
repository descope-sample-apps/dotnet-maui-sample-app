using IBrowser = Duende.IdentityModel.OidcClient.Browser.IBrowser;

namespace DescopeMauiSampleApplication.Descope
{
    public class DescopeClientConfiguration
    {
        public string ProjectId { get; set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public IList<string> Scopes { get; set; } = new string[] { "openid", "profile", "email", "descope.claims", "descope.custom_claims" };

        public string DescopeIssuer { get; set; }

        public IBrowser Browser { get; set; }
    }
}
