using WorkScheduler.Models;
using WorkScheduler.Services;

namespace WorkScheduler.ViewModels
{
    public partial class LogoutViewModel : BindableObject
    {
        private readonly IWorkSchedulerClient _client;
        private readonly INavigationService _navigationService;

        public Command LogoutCommand { get; }

        public LogoutViewModel(IWorkSchedulerClient client, INavigationService navigationService)
        {
            _client = client;
            _navigationService = navigationService;

            LogoutCommand = new(OnLogoutClicked);
        }

        private async void OnLogoutClicked(object _)
        {
            await _client.LogoutAsync();
            await _navigationService.NavigateToLogin();
        }

    }

}
