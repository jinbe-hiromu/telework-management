using System.Collections.ObjectModel;

namespace WorkScheduler.ViewModels;

public class InputDetailsContact
{
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string WorkStyle { get; init; }
    public string WorkingPlace { get; init; }
}

public class InputDetailsViewModel : BindableObject
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
        StartTime = new TimeSpan(8, 40, 0),
        EndTime = new TimeSpan(17, 40, 0),
        WorkStyle = _workStyleGoingToWork,
        WorkingPlace = _workingPlaceAgui,
    };

    public event Action<object> CloseRequested;
    public Size Size { get; } = new Size(500, 400);
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
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