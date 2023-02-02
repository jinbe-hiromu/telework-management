
using CommunityToolkit.Maui.Alerts;
using RestSharp;
using System.Linq.Expressions;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; }

        private RestClient _client = new RestClient("http://localhost:5000");

        public LoginViewModel()
        {
            LoginCommand = new(OnLoginClicked);
        }

        private async void OnLoginClicked(object _)
        {
            try
            {
                var request = new RestRequest($"/api/WorkSchedule/Login");
                request.AddBody("username", UserName);
                request.AddBody("password", Password);
                //var accessToken = await _client.PostAsync(request); 
                var accessToken = "czWjvBwr4eWYX32ZZsJGhw==";
                var parameter = new Dictionary<string, object>() { { "AccessToken", accessToken } };
                await Shell.Current.GoToAsync(nameof(MainPage), parameter);
            }
            catch
            {
                var toast = Toast.Make("ユーザ名かパスワードが間違っています。", CommunityToolkit.Maui.Core.ToastDuration.Short, 14);
                await toast.Show().ConfigureAwait(false);
            }

        }
    }
}
