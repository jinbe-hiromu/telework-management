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
        private RestClient _client = new RestClient("http://localhost:5000");
        private IList<DateTime> _visibleDates;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditScheduleCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteScheduleCommand))]
        private SchedulerAppointment _selectedAppointment;

        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; } = new ObservableCollection<SchedulerAppointment>();

        public bool IsSelected => SelectedAppointment is not null;

        public SchedulerViewModel(CookieContainer cookies)
        {
            var cookie = cookies.GetCookies(new Uri("https://localhost")).First();
            _client.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
        }

        [RelayCommand]
        private void Tapped(SchedulerTappedEventArgs e)
        {
            SelectedAppointment = e?.Appointments is not null ? (SchedulerAppointment)e.Appointments.First() : null;
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
            var result = await Shell.Current.ShowPopupAsync(new InputDetails());
            if (result is InputDetailsContact output)
            {
                await AddEvent(output);
            }
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private async void EditSchedule()
        {
            var info = SchedulerViewModel.SplitSubject(SelectedAppointment.Subject);
            //var view = new InputDetails();
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
                await AddEvent(output);
            }
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private async void DeleteSchedule()
        {
            var request = new RestRequest($"/api/workschedule/{SelectedAppointment.StartTime.Year}/{SelectedAppointment.StartTime.Month}/{SelectedAppointment.StartTime.Day}");

            await _client.DeleteAsync(request);
            await UpdateScheduler(_visibleDates);
        }

        private async Task UpdateScheduler(IList<DateTime> targetDates)
        {
            SchedulerEvents.Clear();

            foreach (var targetDate in targetDates)
            {
                var request = new RestRequest($"/api/workschedule/{targetDate.Year}/{targetDate.Month}/{targetDate.Day}");

                var schedules = await _client.GetAsync<IEnumerable<Schedule>>(request);

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

        private async Task AddEvent(InputDetailsContact output)
        {
            var eventInfo = new EventInfo
            {
                Date = output.Date.ToString("yyyy-MM-dd"),
                StartTime = output.Date.ToString("yyyy-MM-dd") + "T" + output.StartTime.ToString(@"hh\:mm"),        // Format:"2023-01-30T08:40"
                EndTime = output.Date.ToString("yyyy-MM-dd") + "T" + output.EndTime.ToString(@"hh\:mm"),
                WorkStyle = output.WorkStyle,
                WorkingPlace = output.WorkingPlace,
            };

            var request = new RestRequest($"/api/workschedule/{output.Date.Year}/{output.Date.Month}/{output.Date.Day}");
            request.AddBody(eventInfo);

            try
            {
                var response = await _client.PostAsync(request);
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

    internal class EventInfo
    {
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string WorkStyle { get; set; }
        public string WorkingPlace { get; set; }
    }
}
