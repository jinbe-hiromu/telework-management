using WorkScheduler.Views;

namespace WorkScheduler.Services;

public interface INavigationService
{
    Task NavigateTo<T>() where T : Page;
    Task NavigateToDataQuery();
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

    public Task NavigateTo<T>() where T : Page
    {
        var page = ResolvePage<T>();
        return Navigation.PushAsync(page, true);
    }

    public T ResolvePage<T>() where T : Page => _services.GetService<T>();

    public Page ResolvePage(Type type)
    {
        return _services.GetService(type) as Page;
    }
}
