using Syncfusion.Maui.Scheduler;

namespace WorkScheduler.Views;

public partial class SchedulerView : ContentPage
{
    public SchedulerView()
    {
        InitializeComponent();
        Scheduler.DoubleTapped += this.OnSchedulerDoubleTapped;
    }

    private void OnSchedulerDoubleTapped(object sender, SchedulerDoubleTappedEventArgs e)
    {
        var appointments = e.Appointments;
        var selectedDate = e.Date;
        var schedulerElement = e.Element;
    }
}