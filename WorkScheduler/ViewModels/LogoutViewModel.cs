using WorkScheduler.Models;

namespace WorkScheduler.ViewModels
{
    public partial class LogoutViewModel : BindableObject
    {
        private readonly IWorkSchedulerClient _client;

        public Command LogoutCommand { get; }

        public LogoutViewModel(IWorkSchedulerClient client)
        {
            _client = client;

            LogoutCommand = new(OnLogoutClicked);
        }

        private async void OnLogoutClicked(object _)
        {
            //ログアウト処理
            await _client.LogoutAsync();

            await Shell.Current.GoToAsync("..");
        }

    }

}
