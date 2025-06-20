namespace MauiApp1;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
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
