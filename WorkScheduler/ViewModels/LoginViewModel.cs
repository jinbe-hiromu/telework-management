using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using WorkScheduler.Models;
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
        private readonly IWorkSchedulerClient _client;

        public LoginViewModel(INavigationService navigation, IWorkSchedulerClient client)
        {
            _navigation = navigation;
            _client = client;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                await _client.LoginAsync(Username, Password);
                await _navigation.NavigateToMain();
            }
            catch(AuthenticationException)
            {
                var toast = Toast.Make("ユーザ名かパスワードが間違っています。", CommunityToolkit.Maui.Core.ToastDuration.Short, 14);
                await toast.Show().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex); // Do Nothing
            }
        }
    }
}
