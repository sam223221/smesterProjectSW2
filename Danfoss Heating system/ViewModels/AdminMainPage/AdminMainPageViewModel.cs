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
using Microsoft.CodeAnalysis;
using Danfoss_Heating_system.ViewModels.OPT;

namespace Danfoss_Heating_system.ViewModels.AdminMainPage
{
    public partial class AdminMainPageViewModel : ViewModelBase
    {

        private MainWindowViewModel viewchange;


        public AdminMainPageViewModel(MainWindowViewModel mv)
        {
            viewchange = mv;
            viewchange._window.Width = 800;
            viewchange._window.Height = 450-70;
        }

        [RelayCommand]
        private void GoToAdmin()
        {
            viewchange.CurrentContent = new GraphOptimiserView() { DataContext = new GraphOptimiserViewModel(viewchange)};
        }

    }
}
