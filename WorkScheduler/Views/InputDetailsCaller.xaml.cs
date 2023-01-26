using CommunityToolkit.Maui.Views;
using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class InputDetailsCaller : ContentPage
{
	public InputDetailsCaller()
	{
		InitializeComponent();
	}

	public async void OnClicked(object sender, EventArgs e)
	{
		var popup = new InputDetails();
        var result = await this.ShowPopupAsync(popup);
        if (result is InputDetailsContact output)
        {
			var str = $"{nameof(output.Date)}: {output.Date}{Environment.NewLine}{nameof(output.StartTime)}: {output.StartTime}{Environment.NewLine}{nameof(output.EndTime)}: {output.EndTime}{Environment.NewLine}{nameof(output.WorkStyle)}: {output.WorkStyle}{Environment.NewLine}{nameof(output.WorkingPlace)}: {output.WorkingPlace}{Environment.NewLine}";
			var toast = CommunityToolkit.Maui.Alerts.Toast.Make(str);
			_ = toast.Show(CancellationToken.None);
        }
	}
}