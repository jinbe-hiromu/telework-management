using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class LogoutView : ContentPage
{
	public LogoutView(LogoutViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}