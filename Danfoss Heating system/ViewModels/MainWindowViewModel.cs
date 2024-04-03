using CommunityToolkit.Mvvm.ComponentModel;
using Danfoss_Heating_system.ViewModels.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        [ObservableProperty]
        private object currentContent;

        [ObservableProperty]
        private string role ="bob what is this";

        public MainWindowViewModel(string roles) 
        {
            Role = roles;
            
            switch (roles)
            {
                case "Admin":
                    CurrentContent = new AdminView() { DataContext = new AdminMainPageViewModel() };
                    break;
                case "User":
                    CurrentContent = new UserView();
                    break;
                default:
                    // Handle default or unknown role
                    break;
            }
        }
    }
}