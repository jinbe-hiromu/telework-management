using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkScheduler.Services;
using WorkScheduler.Models;

namespace WorkScheduler.ViewModels
{
    public partial class LogoutViewModel : ObservableObject
    {
        private readonly IWorkSchedulerClient _client;
        private readonly INavigationService _navigationService;

        public LogoutViewModel(IWorkSchedulerClient client, INavigationService navigationService)
        {
            _client = client;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Logout()
        {
            await _client.LogoutAsync();
            await _navigationService.NavigateToLogin();
        }
    }
}
