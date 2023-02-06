using WorkScheduler.Views;

namespace WorkScheduler.Services;

public interface INavigationService
{
    Task NavigateToMain();
    Task NavigateToDataQuery();
    Task NavigateTo<T>() where T : Page;
    T ResolvePage<T>() where T : Page;
    Page ResolvePage(Type type);
}

internal class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    protected INavigation Navigation => App.Current.MainPage.Navigation;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public Task NavigateToDataQuery() => NavigateTo<DataQueryView>();

    public Task NavigateToMain() => NavigateTo<MainPage>();

    public Task NavigateTo<T>() where T : Page
    {
        var page = ResolvePage<T>();
        return Navigation.PushAsync(page, true);
    }

    public Page ResolvePage(Type type) => _services.GetService(type) as Page;

    public T ResolvePage<T>() where T : Page => _services.GetService<T>();
}
