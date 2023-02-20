using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkScheduler.Models;

namespace WorkScheduler.ViewModels;

public partial class InputDetailsViewModel : ObservableObject
{
    private static readonly string _workStyleGoingToWork = "出社";
    private static readonly string _workStyleBusinessTrip = "出張";
    private static readonly string _workStyleTelework = "テレワーク";
    private static readonly string _workingPlaceAgui = "阿久比";
    private static readonly string _workingPlaceKariya = "刈谷";
    private static readonly string _workingPlaceAtHome = "自宅";
    private static readonly Dictionary<string, string[]> _workingPlaceSet = new()
    {
        { _workStyleGoingToWork,  new[]{ _workingPlaceAgui,   } },
        { _workStyleBusinessTrip, new[]{ _workingPlaceKariya, } },
        { _workStyleTelework,     new[]{ _workingPlaceAtHome, } },
    };

    private static InputDetailsContact DefaultContact => new()
    {
        StartTime = DateTime.Today + new TimeSpan(8, 40, 0),
        EndTime = DateTime.Today + new TimeSpan(17, 40, 0),
        WorkStyle = _workStyleGoingToWork,
        WorkingPlace = _workingPlaceAgui,
    };

    public event Action<object> CloseRequested;
    public ObservableCollection<string> WorkStyles { get; } = new();
    public ObservableCollection<string> WorkingPlaces { get; } = new();
    public Size Size => new(500, 400);
    public bool IsOkEnabled => StartTime < EndTime;

    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOkEnabled))]
    private TimeSpan _startTime;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOkEnabled))]
    private TimeSpan _endTime;

    [ObservableProperty]
    private string _selectedWorkStyle;

    [ObservableProperty]
    private string _selectedWorkingPlace;

    public InputDetailsViewModel() : this(DefaultContact) { }

    public InputDetailsViewModel(InputDetailsContact contact)
    {
        Date = contact.StartTime.Date;
        StartTime = contact.StartTime.TimeOfDay;
        EndTime = contact.EndTime.TimeOfDay;
        WorkStyles.Add(_workStyleGoingToWork);
        WorkStyles.Add(_workStyleBusinessTrip);
        WorkStyles.Add(_workStyleTelework);
        SelectedWorkStyle = contact.WorkStyle;
        SelectedWorkingPlace = contact.WorkingPlace;
    }

    [RelayCommand]
    private void Ok()
    {
        CloseRequested.Invoke(new InputDetailsContact
        {
            StartTime = Date + StartTime,
            EndTime = Date + EndTime,
            WorkStyle = SelectedWorkStyle,
            WorkingPlace = SelectedWorkingPlace,
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested.Invoke(new());
    }

    partial void OnSelectedWorkStyleChanged(string value)
    {
        if (value is null) { return; }

        WorkingPlaces.Clear();
        foreach (var item in _workingPlaceSet.TryGetValue(value, out var items) ? items : Array.Empty<string>())
        {
            WorkingPlaces.Add(item);
        }

        SelectedWorkingPlace = WorkingPlaces.First();
    }
}