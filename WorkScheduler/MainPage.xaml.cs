using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using System.Threading;

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
	private void OnTempClicked(object sender, EventArgs e)
	{
		var toast = CommunityToolkit.Maui.Alerts.Toast.Make("TEST TOAST!!");
		_ = toast.Show(CancellationToken.None);
	}
}

