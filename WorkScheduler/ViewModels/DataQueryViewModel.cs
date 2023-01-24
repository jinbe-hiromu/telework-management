using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkScheduler.ViewModels;

internal partial class DataQueryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _source;

    public DataQueryViewModel()
    {
        _source = "https://learn.microsoft.com/dotnet/maui";
    }
}
