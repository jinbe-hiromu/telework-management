
namespace WorkScheduler.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new(OnLoginClicked);
        }

        private void OnLoginClicked(object _)
        {
            var toast = CommunityToolkit.Maui.Alerts.Toast.Make("LoginClicked!!");
            _ = toast.Show(CancellationToken.None);
        }


    }
}
