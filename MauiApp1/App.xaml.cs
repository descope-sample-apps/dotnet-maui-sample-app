namespace MauiApp1;

public partial class App : Application
{
	public static string? DeepLinkUrl { get; set; }
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}