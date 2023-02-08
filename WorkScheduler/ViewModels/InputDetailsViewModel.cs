using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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
    public Size Size { get; } = new Size(500, 400);
    public DateTime Date { get; set; }

    private TimeSpan _startTime;
    public TimeSpan StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            CheckSettingTime();
        }
    }

    private TimeSpan _endTime;
    public TimeSpan EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            CheckSettingTime();
        }
    }

    private void CheckSettingTime()
    {
        if (StartTime < EndTime)
        {
            EnableOkCommand = true;
            return;
        }
        EnableOkCommand = false;
    }

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
    [ObservableProperty]
    private bool enableOkCommand;

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
    public Command OkCommand { get; }

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
        OkCommand = new(OnOkClicked);
    }

    private void OnSelectedWorkStyleChanged(string changed)
    {
        if (changed != null)
        {
            WorkingPlaces.Clear();
            foreach (var item in _workingPlaceSet.TryGetValue(changed, out var items) ? items : Array.Empty<string>())
            {
                WorkingPlaces.Add(item);
            }

            SelectedWorkingPlace = WorkingPlaces.First();
        }
    }

    private void OnCancelClicked()
    {
        CloseRequested.Invoke(new());
    }

    private void OnOkClicked()
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
}