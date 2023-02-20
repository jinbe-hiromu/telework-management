namespace WorkScheduler.Services;

public interface INavigationService
{
    Task NavigateToMain();
    Task NavigateToDataQuery();
    Task NavigateTo<T>() where T : Page;
    T ResolvePage<T>() where T : Page;
    Page ResolvePage(Type type);
}
