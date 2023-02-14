using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkScheduler.Models;

namespace WorkScheduler.ViewModels;

public partial class InputDetailsViewModel : ObservableObject
{
    private static readonly string _workStyleGoingToWork = "�o��";
    private static readonly string _workStyleBusinessTrip = "�o��";
    private static readonly string _workStyleTelework = "�e�����[�N";
    private static readonly string _workingPlaceAgui = "���v��";
    private static readonly string _workingPlaceKariya = "���J";
    private static readonly string _workingPlaceAtHome = "����";
    private static readonly Dictionary<string, string[]> _workingPlaceSet = new Dictionary<string, string[]>
    {
        { _workStyleGoingToWork, new[]{ _workingPlaceAgui, } },
        { _workStyleBusinessTrip, new[]{ _workingPlaceKariya, } },
        { _workStyleTelework, new[]{ _workingPlaceAtHome, } },
    };

    private static InputDetailsContact DefaultContact => new InputDetailsContact
    {
        Date = DateTime.Now,
        StartTime = new TimeSpan(8, 40, 0),
        EndTime = new TimeSpan(17, 40, 0),
        WorkStyle = _workStyleGoingToWork,
        WorkingPlace = _workingPlaceAgui,
    };

    public event Action<object> CloseRequested;
    public ObservableCollection<string> WorkStyles { get; } = new();
    public ObservableCollection<string> WorkingPlaces { get; } = new();
    public Size Size { get; } = new Size(500, 400);

    [ObservableProperty]
    public DateTime _date;

    public bool IsOkEnabled => StartTime < EndTime;

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
        Date = contact.Date;
        StartTime = contact.StartTime;
        EndTime = contact.EndTime;
        WorkStyles.Add(_workStyleGoingToWork);
        WorkStyles.Add(_workStyleBusinessTrip);
        WorkStyles.Add(_workStyleTelework);
        SelectedWorkStyle = contact.WorkStyle;
    }

    [RelayCommand]
    private void Ok()
    {
        CloseRequested.Invoke(new InputDetailsContact
        {
            Date = Date,
            StartTime = StartTime,
            EndTime = EndTime,
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
        Debug.WriteLine(value);
        if (value != null)
        {
            WorkingPlaces.Clear();
            foreach (var item in _workingPlaceSet.TryGetValue(value, out var items) ? items : Array.Empty<string>())
            {
                WorkingPlaces.Add(item);
            }

            SelectedWorkingPlace = WorkingPlaces.First();
        }
    }
}