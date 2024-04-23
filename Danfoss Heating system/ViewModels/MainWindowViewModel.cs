using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.ViewModels.AdminMainPage;
using Danfoss_Heating_system.ViewModels.UserMainPage;
using Danfoss_Heating_system.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private Window _window;

        private const double SideBarWidth = 50;

        public ObservableCollection<MyButtonModel> Buttons { get; set; }

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

            Buttons = new ObservableCollection<MyButtonModel>
            {
                new MyButtonModel { ImageSource = "avares://Danfoss_Heating_system/Assets/Images/performance.png", Width = 50, Background = "Blue", ImageHeight = 40, ImageWidth = 40},
                new MyButtonModel { ImageSource = "/Assets/Images/performance.png", Width = 50, Background="Blue", ImageHeight = 40, ImageWidth = 40},
                new MyButtonModel { ImageSource = "/Assets/Images/evaluation.png", Width = 50, Background = "Blue", ImageHeight = 40, ImageWidth = 40},
                new MyButtonModel { ImageSource = "/Assets/Images/performance.png", Width = 50, Background="Transparent", ImageHeight = 40, ImageWidth = 40}
            };
        }
        private bool _isSidebarVisible = true; // Initialize as visible.

        [ObservableProperty]
        private Thickness toggleButtonMargin = new Thickness(SideBarWidth, 0, 0, 0);

        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                if (_isSidebarVisible != value)
                {
                    _isSidebarVisible = value;
                    OnPropertyChanged(); // This calls the OnPropertyChanged method with the caller property name.

                    ToggleButtonMargin = _isSidebarVisible ? new Thickness(SideBarWidth, 0, 0, 0) : new Thickness(0, 0, 0, 0);
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}