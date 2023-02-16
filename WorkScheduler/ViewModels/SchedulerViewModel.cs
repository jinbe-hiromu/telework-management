using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Syncfusion.Maui.Scheduler;
using WorkScheduler.Models;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    public partial class SchedulerViewModel : ObservableObject
    {
        private IList<DateTime> _visibleDates;
        private IWorkSchedulerClient _client;
        private DateTime selectedDateTime;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditScheduleCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteScheduleCommand))]
        private SchedulerAppointment _selectedAppointment;

        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; } = new ObservableCollection<SchedulerAppointment>();

        public bool IsSelected => SelectedAppointment is not null;

        public SchedulerViewModel(IWorkSchedulerClient client)
        {
            _client = client;
        }

        [RelayCommand]
        private void Tapped(SchedulerTappedEventArgs e)
        {
            //SelectedAppointment = e?.Appointments is not null ? (SchedulerAppointment)e.Appointments.First() : null;

            if (e is not null && e.Appointments is not null)
            {
                SelectedAppointment = (SchedulerAppointment)e.Appointments.First();
            }
            else
            {
                SelectedAppointment = null;
                selectedDateTime = (DateTime)e.Date;
            }
        }

        [RelayCommand]
        private void DoubleTapped(SchedulerDoubleTappedEventArgs e)
        {
            SelectedAppointment = e?.Appointments is not null ? (SchedulerAppointment)e.Appointments.First() : null;
            if (SelectedAppointment is not null)
            {
                EditSchedule();
            }
        }

        [RelayCommand]
        private async void ViewChanged(SchedulerViewChangedEventArgs e)
        {
            _visibleDates = e.NewVisibleDates;
            await UpdateScheduler(e.NewVisibleDates);
        }

        [RelayCommand]
        private async void ShowInputDetails()
        {
            //var result = await Shell.Current.ShowPopupAsync(new InputDetails());
            var result = await Shell.Current.ShowPopupAsync(selectedDateTime.Year == 1 ? new InputDetails() : new InputDetails(selectedDateTime));
            if (result is InputDetailsContact output)
            {
                await AddSchedule(output);
            }
            selectedDateTime = DateTime.MinValue;
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private async void EditSchedule()
        {
            var info = SplitSubject(SelectedAppointment.Subject);
            var arg = new InputDetailsContact
            {
                Date = SelectedAppointment.StartTime,
                StartTime = new TimeSpan(SelectedAppointment.StartTime.Hour, SelectedAppointment.StartTime.Minute, SelectedAppointment.StartTime.Second),
                EndTime = new TimeSpan(SelectedAppointment.EndTime.Hour, SelectedAppointment.EndTime.Minute, SelectedAppointment.EndTime.Second),
                WorkStyle = info.WorkStyle,
                WorkingPlace = info.WorkingPlace
            };

            var result = await Shell.Current.ShowPopupAsync(new InputDetails(arg));
            if (result is InputDetailsContact output)
            {
                await AddSchedule(output);
            }
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private async void DeleteSchedule()
        {
            await _client.DeleteScheduleAsync(SelectedAppointment.StartTime);
            await UpdateScheduler(_visibleDates);
        }

        private async Task UpdateScheduler(IList<DateTime> targetDates)
        {
            SchedulerEvents.Clear();

            foreach (var targetDate in targetDates)
            {
                var schedules = await _client.GetScheduleAsync(targetDate);

                foreach (var schedule in schedules)
                {
                    if (schedule.ScheduleType == ScheduleType.Plan)
                    {
                        SchedulerEvents.Add(new SchedulerAppointment
                        {
                            StartTime = schedule.StartTime,
                            EndTime = schedule.EndTime,
                            Subject = CreateSubject(schedule.WorkStyle, schedule.WorkingPlace),
                            Background = new SolidColorBrush(Colors.LightBlue)
                        });
                    }
                    else
                    {
                        SchedulerEvents.Add(new SchedulerAppointment
                        {
                            StartTime = schedule.StartTime,
                            EndTime = schedule.EndTime,
                            Subject = CreateSubject(schedule.WorkStyle, schedule.WorkingPlace),
                            Background = new SolidColorBrush(Colors.LightGoldenrodYellow)
                        });
                    }
                }
            }
        }

        private async Task AddSchedule(InputDetailsContact output)
        {
            try
            {
                await _client.AddScheduleAsync(output);
                await UpdateScheduler(_visibleDates);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static string CreateSubject(string workStyle, string workingPlace)
        {
            if (string.IsNullOrEmpty(workingPlace))
            {
                return $"{workStyle}";
            }
            return $"{workStyle}[{workingPlace}]";
        }

        private static EventInfo SplitSubject(string subject)
        {
            var workStyle = subject.Substring(0, subject.IndexOf('['));
            var workingPlace = subject.Substring(subject.IndexOf('[') + 1, subject.IndexOf(']') - subject.IndexOf('[') - 1);

            return new EventInfo { WorkStyle = workStyle, WorkingPlace = workingPlace };
        }
    }
}
