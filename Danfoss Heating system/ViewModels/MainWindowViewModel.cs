﻿using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using Danfoss_Heating_system.ViewModels.AdminMainPage;
using Danfoss_Heating_system.ViewModels.OPT;
using Danfoss_Heating_system.ViewModels.TopBarNavigation;
using Danfoss_Heating_system.ViewModels.UserMainPage;
using Danfoss_Heating_system.Views;
using Danfoss_Heating_system.Views.TopBarNavigation;
using ReactiveUI;
using System.Collections.ObjectModel;


namespace Danfoss_Heating_system.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public Window window;

        [ObservableProperty]
        private double sideBarWidth = 0;

        [ObservableProperty]
        private double actualWidth = 0;

        [ObservableProperty]
        private double actualHeight = 0;

        public EnergyData userName;


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

        [RelayCommand]
        private void GoHome()
        {
            if (userName.UserRole == "Admin")
            {
                window.Width = 800;
                window.Height = 450;
                CurrentContent = new AdminView() { DataContext = new AdminMainPageViewModel(this) };
            }
        }


        public MainWindowViewModel(EnergyData item, Window window)
        {

            this.window = window;
            userName = item;


            //sets the view within the window to the current role that is logging in
            switch (item.UserRole)
            {
                case "Admin":
                    CurrentContent = new AdminView() { DataContext = new AdminMainPageViewModel(this) };
                    AdminNavigationBar();
                    break;
                case "User":
                    CurrentContent = new UserView() { DataContext = new UserMainPageViewModel(this) };
                    break;
            }

        }

        private void AdminNavigationBar()
        {
            NavigationButtons = new ObservableCollection<MyButtonModel>
            {
                new MyButtonModel("GraphOPT", ReactiveCommand.Create<string>(AdminNavigationExe)),
                new MyButtonModel("LiveOPT", ReactiveCommand.Create<string>(AdminNavigationExe)),
            };
        }

        private void AdminNavigationExe(string parameter)
        {

            switch (parameter)
            {
                case "GraphOPT":
                    CurrentContent = new GraphOptimiserView() { DataContext = new GraphOptimiserViewModel(this) };
                    break;
                case "LiveOPT":
                    CurrentContent = new LiveOptimiser() { DataContext = new LiveOptimiserViewModel(this) };
                    break;

            }

            _isSideBarOpen = false;
            SideBarWidth = 0;
        }



        [RelayCommand]
        private void SideBarToggle()
        {

            _isSideBarOpen = !_isSideBarOpen;
            SideBarWidth = _isSideBarOpen ? 75 : 0;

        }

        [RelayCommand]
        private void FAQButton()
        {
            window.Width = 800;
            window.Height = 450;
            CurrentContent = new FAQView() { DataContext = new FAQViewModel() };
        }

        [RelayCommand]
        private void UnitsButton()
        {
            window.Width = 800;
            window.Height = 450;
            CurrentContent = new UnitsView() { DataContext = new UnitsViewModel() };
        }

        [RelayCommand]
        private void HelpButton()
        {
            window.Width = 800;
            window.Height = 450;
            CurrentContent = new HelpView() { DataContext = new HelpViewModel() };
        }

    }
}