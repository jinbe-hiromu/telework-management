using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Scheduler;
using WorkScheduler.Models;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    public partial class SchedulerViewModel : ObservableObject
    {
        private readonly IWorkSchedulerClient _client;
        private IList<DateTime> _visibleDates;

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
            SelectedAppointment = e?.Appointments is not null ? (SchedulerAppointment)e.Appointments.First() : null;
        }

        [RelayCommand]
        private void DoubleTapped(SchedulerDoubleTappedEventArgs e)
        {
            SelectedAppointment = e?.Appointments is not null ? (SchedulerAppointment)e.Appointments.First() : null;
            if (IsSelected)
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
                await AddSchedule(output);
            }
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private async void EditSchedule()
        {
            var info = SplitSubject(SelectedAppointment.Subject);
            var arg = new InputDetailsContact
            {
                StartTime = SelectedAppointment.StartTime,
                EndTime = SelectedAppointment.EndTime,
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
            SelectedAppointment = null;
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
                    var bgColor = schedule.ScheduleType == ScheduleType.Plan ? Colors.LightBlue : Colors.LightGoldenrodYellow;

                    SchedulerEvents.Add(new SchedulerAppointment
                    {
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        Subject = CreateSubject(schedule.WorkStyle, schedule.WorkingPlace),
                        Background = new SolidColorBrush(bgColor)
                    });
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
