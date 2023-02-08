using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkScheduler.ViewModels;

public partial class DataQueryViewModel : ObservableObject
{
    [ObservableProperty]
    private CookieContainer _cookies;
    [ObservableProperty]
    private UrlWebViewSource _source;

    public DataQueryViewModel(CookieContainer cookies)
    {
        _cookies = cookies;
        _source = new UrlWebViewSource
        {
            Url = $"http://localhost:5000/api/WorkSchedule/2023/2/3?accesskey={cookies.GetCookieHeader(new Uri("https://localhost:5001"))}"
        };
    }
}
