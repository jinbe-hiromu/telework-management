using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
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
        private SchedulerAppointment _selectedAppointment;

        public SchedulerViewModel(CookieContainer cookies)
        {
            TappedCommand = new Command<SchedulerTappedEventArgs>(OnSchedulerTapped);
            OnViewChangedCommand = new Command<SchedulerViewChangedEventArgs>(OnVeiwChangedAsync);

            var cookie = cookies.GetCookies(new Uri("http://localhost")).First();
            _client.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
        }

        private Command<SchedulerTappedEventArgs> _tappedCommand;

        public Command<SchedulerTappedEventArgs> TappedCommand
        {
            get { return _tappedCommand; }
            set { _tappedCommand = value; }
        }

        private Command<SchedulerViewChangedEventArgs> _onViewChangedCommand;

        public Command<SchedulerViewChangedEventArgs> OnViewChangedCommand
        {
            get { return _onViewChangedCommand; }
            set { _onViewChangedCommand = value; }
        }

        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; } = new ObservableCollection<SchedulerAppointment>();

        private void OnSchedulerTapped(SchedulerTappedEventArgs e)
        {
            if (e is not null && e.Appointments is not null)
            {
                _selectedAppointment = (SchedulerAppointment)e.Appointments.First();
            }
            else
            {
                _selectedAppointment = null;
            }
        }

        private async void OnVeiwChangedAsync(SchedulerViewChangedEventArgs e)
        {
            if (e is not null)
            {
                _visibleDates = e.NewVisibleDates;
                await UpdateScheduler(e.NewVisibleDates);
            }
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

        [RelayCommand]
        private async void ShowInputDetails()
        {
            var result = await Shell.Current.ShowPopupAsync(new InputDetails());
            if (result is InputDetailsContact output)
            {
                await AddEvent(output);
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

        [RelayCommand]
        private async void EditSchedule()
        {
            var info = DivideSubject(_selectedAppointment.Subject);
            //var view = new InputDetails();
            var arg = new InputDetailsContact
            {
                Date = _selectedAppointment.StartTime,
                StartTime = new TimeSpan(_selectedAppointment.StartTime.Hour, _selectedAppointment.StartTime.Minute, _selectedAppointment.StartTime.Second),
                EndTime = new TimeSpan(_selectedAppointment.EndTime.Hour, _selectedAppointment.EndTime.Minute, _selectedAppointment.EndTime.Second),
                WorkStyle = info.WorkStyle,
                WorkingPlace = info.WorkingPlace
            };

            var result = await Shell.Current.ShowPopupAsync(new InputDetails(arg));
            if (result is InputDetailsContact output)
            {
                await AddEvent(output);
            }
        }


        [RelayCommand]
        private async void DeleteSchedule()
        {
            if (_selectedAppointment is not null)
            {
                var request = new RestRequest($"/api/workschedule/{_selectedAppointment.StartTime.Year}/{_selectedAppointment.StartTime.Month}/{_selectedAppointment.StartTime.Day}");

                await _client.DeleteAsync(request);
                await UpdateScheduler(_visibleDates);
            }
        }

        private string CreateSubject(string workStyle, string workingPlace)
        {
            if (string.IsNullOrEmpty(workingPlace))
            {
                return $"{workStyle}";
            }
            return $"{workStyle}[{workingPlace}]";
        }

        private EventInfo DivideSubject(string subject)
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
