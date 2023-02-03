using System.Diagnostics;
using System.Net;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using RestSharp;
using WorkScheduler.Services;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;
        [ObservableProperty]
        private string _password = string.Empty;

        private readonly RestClient _client;
        private readonly INavigationService _navigation;
        private readonly CookieContainer _cookies;

        public Command LoginCommand { get; }

        public LoginViewModel() => throw new InvalidOperationException();

        public LoginViewModel(INavigationService navigation, CookieContainer cookies)
        {
            _client = new RestClient("http://localhost:5000");
            _navigation = navigation;
            _cookies = cookies;
            LoginCommand = new(OnLoginClicked);
        }

        private async void OnLoginClicked()
        {
            var request = new RestRequest($"/account/login?redirectUrl=", Method.Post);
            request.AddParameter("Username", Username);
            request.AddParameter("Password", Password);

            try
            {
                var response = _client.PostAsync(request).Result;

                if (response.StatusCode == HttpStatusCode.OK && response.Cookies.Count > 0)
                {
                    _cookies.Add(new Uri("http://localhost"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000/api"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000/api/WorkSchedule"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000/api/WorkSchedule/2023"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000/api/WorkSchedule/2023/2"), response.Cookies);
                    _cookies.Add(new Uri("http://localhost:5000/api/WorkSchedule/2023/2/3"), response.Cookies);
                    await _navigation.NavigateTo<MainPage>();
                }
                else
                {
                    var toast = Toast.Make("ユーザ名かパスワードが間違っています。", CommunityToolkit.Maui.Core.ToastDuration.Short, 14);
                    await toast.Show().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex); // Do Nothing
            }
        }
    }
}
