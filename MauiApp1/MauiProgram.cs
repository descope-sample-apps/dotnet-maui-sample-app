using Descope.Maui;
using Microsoft.Extensions.Logging;

namespace MauiApp1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// TODO: Replace with your Descope project ID, and allow the redirect URI in your project
		builder.Services.AddDescope(options =>
		{
			options.ProjectId = "<YOUR_DESCOPE_PROJECT_ID>";
			options.RedirectUri = "descopemaui://auth";
			// Optional: sign in through a specific OIDC application instead of the project's default one
			// options.ClientId = "<OIDC_APPLICATION_CLIENT_ID>";
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
