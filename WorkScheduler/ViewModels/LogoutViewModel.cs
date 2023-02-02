using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkScheduler.Views;

namespace WorkScheduler.ViewModels
{
    class LogoutViewModel : BindableObject
    {
        public Command LogoutCommand { get; }

        public LogoutViewModel()
        {
            LogoutCommand = new(OnLogoutClicked);
        }

        private async void OnLogoutClicked(object _)
        {
            //ログアウト処理
            await Shell.Current.GoToAsync("..");
        }

    }

}
