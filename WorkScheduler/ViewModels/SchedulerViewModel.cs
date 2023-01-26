using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    [QueryProperty(nameof(InputData), "SchedulerAppointment")]
    internal partial class SchedulerViewModel : ObservableObject
    {
        public SchedulerAppointment InputData
        {
            set
            {
                SchedulerEvents.Add(value);
            }
        }

        private Command<SchedulerTappedEventArgs> _tappedCommand;

        public Command<SchedulerTappedEventArgs> TappedCommand
        {
            get { return _tappedCommand; }
            set { _tappedCommand = value; }
        }

        //private Command _tappedCommand;

        //public Command TappedCommand
        //{
        //    get { return _tappedCommand; }
        //    set { _tappedCommand = value; }
        //}

        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; } = new ObservableCollection<SchedulerAppointment>();

        public SchedulerViewModel()
        {
            TappedCommand = new Command<SchedulerTappedEventArgs>(OnSchedulerTapped);
        }

        [RelayCommand]
        private async void AddNewSchedule()
        {
            await Shell.Current.GoToAsync(nameof(InputDetails));
        }

        private void OnSchedulerTapped(SchedulerTappedEventArgs e)
        {
            if (e is not null)
            {
                var appointments = e.Appointments;
                var selectedDate = e.Date;
                var schedulerElement = e.Element;
            }
        }

        //private void OnSchedulerTapped(object sender)
        //{
        //    if (e is not null)
        //    {
        //        var appointments = e.Appointments;
        //        var selectedDate = e.Date;
        //        var schedulerElement = e.Element;
        //    }
        //}

        //private void OnSchedulerTapped(object obj)
        //{
        //    if (obj is not null)
        //    {
        //        var eventArgs = obj as SchedulerTappedEventArgs;
        //        var appointments = eventArgs.Appointments;
        //        var selectedDate = eventArgs.Date;
        //        var schedulerElement = eventArgs.Element;
        //    }
        //}
    }
}
