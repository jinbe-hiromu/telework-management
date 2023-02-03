using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class DataQueryView : ContentPage
{
	public DataQueryView(DataQueryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}