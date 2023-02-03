using WorkScheduler.ViewModels;

namespace WorkScheduler.Views;

public partial class SchedulerView : ContentPage
{
    public SchedulerView(SchedulerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}