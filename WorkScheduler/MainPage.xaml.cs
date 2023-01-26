using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using System.Threading;
using WorkScheduler.Views;

namespace WorkScheduler;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
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

	// DELETE ME
	private async void OnTempClicked(object sender, EventArgs e)
	{
		await Shell.Current.Navigation.PushModalAsync(new InputDetailsCaller());

	}
}

