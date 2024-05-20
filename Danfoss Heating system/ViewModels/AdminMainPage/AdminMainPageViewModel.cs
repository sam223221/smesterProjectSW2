using System;
using System.Collections.Generic;
using System.Linq;
using Danfoss_Heating_system.ViewModels;
using System.Text;
using System.Threading.Tasks;
using Danfoss_Heating_system.Views;
using Danfoss_Heating_system.ViewModels.UserMainPage;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Interactivity;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using DocumentFormat.OpenXml.Packaging;
using Danfoss_Heating_system.ViewModels.OPT;

namespace Danfoss_Heating_system.ViewModels.AdminMainPage
{
    public partial class AdminMainPageViewModel : ViewModelBase
    {

        private MainWindowViewModel viewchange;

        [ObservableProperty]
        private string userName;

        public AdminMainPageViewModel(MainWindowViewModel mv)
        {
            viewchange = mv;
            viewchange.window.Width = 800;
            viewchange.window.Height = 450;
            viewchange.window.CanResize = false;
            userName = "welcome back " + mv.userName.UserID;
        }


        [RelayCommand]
        private void GoToUser()
        {
            
        }

        [RelayCommand]
        private void LiveOptimiser()
        {
            viewchange.CurrentContent = new LiveOptimiser() { DataContext = new LiveOptimiserViewModel(viewchange) };
        }

    }
}
