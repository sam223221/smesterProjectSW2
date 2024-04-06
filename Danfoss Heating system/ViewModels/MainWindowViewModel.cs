using Avalonia.Controls;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.ViewModels.AdminMainPage;
using Danfoss_Heating_system.ViewModels.UserMainPage;
using Danfoss_Heating_system.Views;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private Window _window;


        [ObservableProperty]
        private object currentContent;

        [RelayCommand]
        private void logout()
        {

            var loginWindow = new LoginWindow();
            loginWindow.DataContext = new LoginWindowViewModel(loginWindow); // Passes the window so it can be manipulated

            loginWindow.Show();
            _window.Close();
        }

        public MainWindowViewModel(string roles, Window window) 
        {

            _window = window;


            //sets the view within the window to the current role that is logging in
            switch (roles)
            {
                case "Admin":
                    CurrentContent = new AdminView() { DataContext = new AdminMainPageViewModel(this) };
                    break;
                case "User":
                    CurrentContent = new UserView() { DataContext = new UserMainPageViewModel(this) };
                    break;
            }
        }
    }
}