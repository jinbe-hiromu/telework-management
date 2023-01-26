using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduler.ViewModels
{
    internal partial class HamburgerMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _schedulePage;

        public HamburgerMenuViewModel()
        {

        }

        //public Command RogOutCommand { get; }
        //private Command<Button> _rogOutCommand;

    }
}
