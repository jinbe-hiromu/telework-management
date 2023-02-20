using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkScheduler.Services;

namespace WorkScheduler.ViewModels
{
    public partial class LogoutViewModel : ObservableObject
    {
        private readonly IWorkSchedulerClient _client;

        public LogoutViewModel(IWorkSchedulerClient client)
        {
            _client = client;
        }

        [RelayCommand]
        private async Task Logout()
        {
            //ログアウト処理
            await _client.LogoutAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}
