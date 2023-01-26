using CommunityToolkit.Maui.Views;
using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class InputDetails : Popup
{
	public InputDetails()
	{
		InitializeComponent();
		var vm = BindingContext as InputDetailsViewModel;
		vm.CloseRequested += Close;
	}
}