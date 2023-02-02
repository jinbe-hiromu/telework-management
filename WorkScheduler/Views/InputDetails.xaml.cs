using CommunityToolkit.Maui.Views;
using WorkScheduler.Models;
using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class InputDetails : Popup
{
    public InputDetails()
    {
        InitializeComponent();
        BindingContext = new InputDetailsViewModel();
        var vm = BindingContext as InputDetailsViewModel;
        vm.CloseRequested += Close;
    }

    public InputDetails(InputDetailsContact arg)
    {
        InitializeComponent();
        BindingContext = new InputDetailsViewModel(arg);
        var vm = BindingContext as InputDetailsViewModel;
        vm.CloseRequested += Close;
    }
}