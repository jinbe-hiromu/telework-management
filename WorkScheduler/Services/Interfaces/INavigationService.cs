using WorkScheduler.Models;
using WorkScheduler.Views;

namespace WorkScheduler.Services;

public interface INavigationService
{
    Task NavigateToMain();
    Task NavigateToDataQuery();
    Task NavigateToLogin();
    Task<object> NavigateToInputDetail(InputDetailsContact parameter);
    Task NavigateTo<T>() where T : Page;
    T ResolvePage<T>() where T : Page;
    Page ResolvePage(Type type);
}
