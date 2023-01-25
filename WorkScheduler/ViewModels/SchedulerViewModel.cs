using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduler.ViewModels
{
    internal partial class SchedulerViewModel : ObservableObject
    {
        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; } = new ObservableCollection<SchedulerAppointment>();

        [ObservableProperty]
        private DateTime _displayDate;

        [ObservableProperty]
        private DateTime _selectedDate;


        public SchedulerViewModel()
        {
        }

        [RelayCommand]
        private void AddNewSchedule()
        {
            SchedulerEvents.Add(new SchedulerAppointment { StartTime = new DateTime(2023, 1, 25, 18, 0, 0), EndTime = new DateTime(2023, 1, 25, 20, 0, 0), Subject = "テスト", Background = Colors.Green });
        }

        public void OnSchedulerDoubleTapped(object sender, SchedulerDoubleTappedEventArgs e)
        {
            var appointments = e.Appointments;
            var selectedDate = e.Date;
            var schedulerElement = e.Element;
        }
    }
}
