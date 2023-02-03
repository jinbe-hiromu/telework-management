using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkScheduler.ViewModels;

public partial class DataQueryViewModel : ObservableObject
{
    [ObservableProperty]
    private UrlWebViewSource _source;
    [ObservableProperty]
    private CookieContainer _cookies;

    public DataQueryViewModel() : this(new CookieContainer()) { }

    public DataQueryViewModel(CookieContainer cookies)
    {
        _source = new UrlWebViewSource { Url = $"http://localhost:5000/api/WorkSchedule/2023/2/3?accesskey={cookies.GetCookieHeader(new Uri("http://localhost:5000"))}" };
        _cookies = cookies;
    }
}
