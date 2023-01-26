using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using WorkScheduler.Views;
using SchedulerView = WorkScheduler.Views.SchedulerView;

namespace WorkScheduler.ViewModels;

//public interface IInputDetails
//{
//	public DateTime Date { get; }
//	public DateTime StartTime { get; } 
//	public DateTime EndTime { get; } 
//	public string WorkStyle { get; }  
//	public string WorkingPlace { get; }
//}

public class InputDetailsContact
{
    public DateTime Date { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string WorkStyle { get; init; }
    public string WorkingPlace { get; init; }
}


public partial class InputDetailsViewModel : BindableObject
{
    private static readonly string _workStyleGoingToWork = "出社";
    private static readonly string _workStyleBusinessTrip = "出張";
    private static readonly string _workStyleTelework = "テレワーク";
    private static readonly string _workingPlaceAgui = "阿久比";
    private static readonly string _workingPlaceKariya = "刈谷";
    private static readonly string _workingPlaceAtHome = "自宅";
    private static readonly Dictionary<string, string[]> _workingPlaceSet = new Dictionary<string, string[]>
    {
        { _workStyleGoingToWork, new[]{ _workingPlaceAgui, } },
        { _workStyleBusinessTrip, new[]{ _workingPlaceKariya, } },
        { _workStyleTelework, new[]{ _workingPlaceAtHome, } },
    };

    private static InputDetailsContact DefaultContact => new InputDetailsContact
    {
        Date = DateTime.Now,
        StartTime = new DateTime(2023, 1, 25, 8, 40, 0),
        EndTime = new DateTime(2023, 1, 25, 17, 40, 0),
        WorkStyle = _workStyleGoingToWork,
        WorkingPlace = _workingPlaceAgui,
    };
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ObservableCollection<string> WorkStyles { get; } = new();
    private string _selectedWorkStyle;
    public string SelectedWorkStyle
    {
        get => _selectedWorkStyle;
        set
        {
            _selectedWorkStyle = value;
            OnSelectedWorkStyleChanged(value);
        }
    }

    public ObservableCollection<string> WorkingPlaces { get; } = new();
    private string _selectedWorkingPlace;
    public string SelectedWorkingPlace
    {
        get => _selectedWorkingPlace;
        set
        {
            _selectedWorkingPlace = value;
            OnPropertyChanged();
        }
    }

    public Command CancelCommand { get; }
    //public Command OkCommand { get; }

    public InputDetailsViewModel() : this(DefaultContact) { }
    public InputDetailsViewModel(InputDetailsContact contact)
    {
        Date = contact.Date;
        StartTime = contact.StartTime;
        EndTime = contact.EndTime;
        WorkStyles.Add(_workStyleGoingToWork);
        WorkStyles.Add(_workStyleBusinessTrip);
        WorkStyles.Add(_workStyleTelework);
        SelectedWorkStyle = contact.WorkStyle;
        CancelCommand = new(OnCancelClicked);
        //OkCommand = new(OnOkClicked);
    }

    private void OnSelectedWorkStyleChanged(string changed)
    {
        WorkingPlaces.Clear();
        foreach (var item in _workingPlaceSet.TryGetValue(changed, out var items) ? items : Array.Empty<string>())
        {
            WorkingPlaces.Add(item);
        }

        SelectedWorkingPlace = WorkingPlaces.First();
    }

    private void OnCancelClicked(object _)
    {

    }

    [RelayCommand]
    private async void OnOkClicked(object _)
    {
        var startTime = new DateTime(Date.Year, Date.Month, Date.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);
        var endTime = new DateTime(Date.Year, Date.Month, Date.Day, EndTime.Hour, EndTime.Minute, EndTime.Second);
        var subject = $"{SelectedWorkStyle}({SelectedWorkingPlace})";

        var schedulerAppointment = new SchedulerAppointment
        {
            StartTime = startTime,
            EndTime = endTime,
            Subject = subject,
        };

        await Shell.Current.GoToAsync("///SchedulerView", new Dictionary<string, object> { { "SchedulerAppointment", schedulerAppointment } });
    }
}