using CommunityToolkit.Maui.Views;
using WorkScheduler.Models;
using WorkScheduler.Views;

namespace WorkScheduler.Services;

public interface INavigationService
{
    Task NavigateToLogin();
    Task NavigateToMain();
    Task NavigateToDataQuery();
    Task NavigateTo<T>() where T : Page;
    Page ResolvePage(Type type);
    T ResolvePage<T>() where T : Page;
}

internal class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    protected INavigation Navigation => App.Current.MainPage.Navigation;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public Task NavigateToLogin() => NavigateTo<LoginView>();

    public Task NavigateToMain() => NavigateTo<MainPage>();

    public Task NavigateToDataQuery() => NavigateTo<DataQueryView>();

    //public Task<InputDetailsContact> NavigateToInputDetails(InputDetails)
    //{
    //    NavigateTo<>
    //    //if(DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
    //    //{
    //    //    return PopupTo<>
    //    //}
    //    //else
    //    //{

    //    //}
    //}


    public Task NavigateTo<T>() where T : Page
    {
        var page = ResolvePage<T>();
        return Navigation.PushAsync(page, true);
    }

    public Task PopupTo<T>() where T : Popup
    {
        var popup = ResolvePopup<T>();
        return Navigation.PopAsync(popup);
    }

    public Page ResolvePage(Type type) => _services.GetService(type) as Page;

    public T ResolvePage<T>() where T : Page => _services.GetService<T>();

    public T ResolvePopup<T>() where T : Popup => _services.GetService<T>();
}
