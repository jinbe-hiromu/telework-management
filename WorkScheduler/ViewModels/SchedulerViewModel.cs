using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    internal partial class SchedulerViewModel : ObservableObject
    {
        private string _accessToken = "czWjvBwr4eWYX32ZZsJGhw==";
        private RestClient _client = new RestClient("http://localhost:5000");

        public SchedulerViewModel()
        {
            TappedCommand = new Command<SchedulerTappedEventArgs>(OnSchedulerTapped);
            OnViewChangedCommand = new Command<SchedulerViewChangedEventArgs>(OnVeiwChangedAsync);
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            SchedulerEvents.Add(new SchedulerAppointment
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Subject = "test"
            });
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
            if (e is not null)
            {
                var appointments = e.Appointments;
                var selectedDate = e.Date;
                var schedulerElement = e.Element;
            }
        }

        private async void OnVeiwChangedAsync(SchedulerViewChangedEventArgs e)
        {
            if (e is not null)
            {
                foreach (var visibleDate in e.NewVisibleDates)
                {
                    var request = new RestRequest($"/api/workschedule/{visibleDate.Year}/{visibleDate.Month}/{visibleDate.Day}");
                    request.AddHeader("AccessToken", _accessToken);

                    var response = await _client.GetAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var toast = CommunityToolkit.Maui.Alerts.Toast.Make("StatusCode:" + response.StatusCode);
                        _ = toast.Show(CancellationToken.None);
                        continue;
                    }

                    //var jsonString = response.Content.ReadAsStringAsync().Result;

                    //var eventData = JsonSerializer.Deserialize<EventInfo>(jsonString);

                    //SchedulerEvents.Add(new SchedulerAppointment
                    //{
                    //    StartTime = DateTime.Parse(eventData.StartTime),
                    //    EndTime = DateTime.Parse(eventData.EndTime),
                    //    Subject = $"{eventData.WorkStyle}[{eventData.WorkingPlace}]",
                    //});
                }
            }
        }

        [RelayCommand]
        private async void ShowInputDetails()
        {
            var result = await Shell.Current.ShowPopupAsync(new InputDetails());
            if (result is InputDetailsContact output)
            {
                var str = $"{nameof(output.Date)}: {output.Date}{Environment.NewLine}{nameof(output.StartTime)}: {output.StartTime}{Environment.NewLine}{nameof(output.EndTime)}: {output.EndTime}{Environment.NewLine}{nameof(output.WorkStyle)}: {output.WorkStyle}{Environment.NewLine}{nameof(output.WorkingPlace)}: {output.WorkingPlace}{Environment.NewLine}";


                var requestBody = new EventInfo
                {
                    Date = output.Date.ToString("yyyy-MM-dd"),
                    StartTime = output.Date.ToString("yyyy-MM-dd") + "T" + output.StartTime.ToString(@"hh\:mm"),        // Format:"2023-01-30T08:40"
                    EndTime = output.Date.ToString("yyyy-MM-dd") + "T" + output.EndTime.ToString(@"hh\:mm"),
                    WorkStyle = output.WorkStyle,
                    WorkingPlace = output.WorkingPlace,
                };
                //var response = await _client.PostAsJsonAsync($"{_host}/api/workschedule/{output.Date.Year}/{output.Date.Month}/{output.Date.Day}", requestBody);


                //if (!response.IsSuccessStatusCode)
                //{
                //    var toast = CommunityToolkit.Maui.Alerts.Toast.Make("StatusCode:" + response.StatusCode);
                //    _ = toast.Show(CancellationToken.None);
                //}
            }
        }


        [RelayCommand]
        private void EditScheduler()
        {

        }


        [RelayCommand]
        private void DeleteScheduler()
        {

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
