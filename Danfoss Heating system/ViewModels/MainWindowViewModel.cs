using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using Danfoss_Heating_system.ViewModels.AdminMainPage;
using Danfoss_Heating_system.ViewModels.UserMainPage;
using Danfoss_Heating_system.Views;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Windows.Input;


namespace Danfoss_Heating_system.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private Window window;

        [ObservableProperty]
        private int sideBarWidth = 0;


        private bool _isSideBarOpen = false;

        public ObservableCollection<MyButtonModel> NavigationButtons { get; set; }


        [ObservableProperty]
        private object currentContent;

        [RelayCommand]
        private void logout()
        {

            var loginWindow = new LoginWindow();
            loginWindow.DataContext = new LoginWindowViewModel(loginWindow); // Passes the window so it can be manipulated
            
            loginWindow.Show();
            window.Close();
        }




        public MainWindowViewModel(string roles, Window window)
        {

            this.window = window;




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

            NavigationButtons = new ObservableCollection<MyButtonModel>
            {
                new MyButtonModel { ButtonName = "Admin"}
            };

        }




        [RelayCommand]
        private void SideBarToggle() 
        {

            _isSideBarOpen = !_isSideBarOpen;
            
            
            if (_isSideBarOpen)
            {
                SideBarWidth = 56;
            }else
            {
                SideBarWidth = 0;
            }
                
        }


    }
}