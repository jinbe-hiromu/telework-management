using System.Diagnostics;
using System.Net;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using WorkScheduler.Services;

namespace WorkScheduler.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;
        [ObservableProperty]
        private string _password = string.Empty;

        private readonly INavigationService _navigation;
        private readonly RestClient _client;
        private readonly CookieContainer _cookies;

        public LoginViewModel(INavigationService navigation, CookieContainer cookies)
        {
            _navigation = navigation;
            _client = new RestClient("http://localhost:5000");
            _cookies = cookies;
        }

        [RelayCommand]
        private async Task LoginAsync()
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
                    await _navigation.NavigateToMain();
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
