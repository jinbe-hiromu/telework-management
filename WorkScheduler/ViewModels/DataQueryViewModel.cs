using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkScheduler.ViewModels;

public partial class DataQueryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _cookie;

    [ObservableProperty]
    private string _url;

    public DataQueryViewModel(CookieContainer cookies)
    {
        _cookie = cookies.GetCookieHeader(new Uri("http://localhost"));
        _url = "http://localhost:5000/";
    }
}
