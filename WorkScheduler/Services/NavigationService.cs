using CommunityToolkit.Maui.Views;
using WorkScheduler.Models;
using WorkScheduler.ViewModels;
using WorkScheduler.Views;


namespace WorkScheduler.Services;

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

    public Task NavigateToLogin() => NavigateTo<LoginView>();

    public async Task<object> NavigateToInputDetail(InputDetailsContact parameter)
    {
        var popup = _services.GetService<InputDetails>();
        var vm = popup.BindingContext as InputDetailsViewModel;
        vm.Initialize(parameter);
        return await Shell.Current.ShowPopupAsync(popup);
    } 

    public Task NavigateTo<T>() where T : Page
    {
        var page = ResolvePage<T>();
        return Navigation.PushAsync(page, true);
    }

    public Page ResolvePage(Type type) => _services.GetService(type) as Page;

    public Popup ResolvePopup(Type type) => _services.GetService(type) as Popup;

    public T ResolvePage<T>() where T : Page => _services.GetService<T>();
}
