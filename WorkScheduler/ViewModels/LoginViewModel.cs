
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new(OnLoginClicked);
        }

        private async void OnLoginClicked(object _)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
    }
}
