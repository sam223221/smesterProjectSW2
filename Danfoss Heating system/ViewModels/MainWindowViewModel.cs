using CommunityToolkit.Mvvm.ComponentModel;
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

        public MainWindowViewModel(string roles) 
        {
            //sets the view within the window to the current role that is logging in
            switch (roles)
            {
                case "Admin":
                    CurrentContent = new AdminView();
                    break;
                case "User":
                    CurrentContent = new UserView();
                    break;
            }
        }
    }
}