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
        }

        [RelayCommand]
        private void GoToUser()
        {
            viewchange.CurrentContent = new GraphOptimiserView() { DataContext = new GraphOptimiserViewModel(viewchange)};
        }

    }
}
