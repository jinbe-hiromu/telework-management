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
        _cookies = cookies.Copy();
        _source = new UrlWebViewSource
        {
            Url = "http://localhost:5000/work-schedule-view"
        };
    }
}

internal static class CookieContainerExtensions
{
    internal static CookieContainer Copy(this CookieContainer source)
    {
        var destination = new CookieContainer(source.Capacity,
                                              source.PerDomainCapacity,
                                              source.MaxCookieSize);

        var cookies = source.GetAllCookies();
        destination.Add(cookies);

        return destination;
    }
}