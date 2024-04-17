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

namespace Danfoss_Heating_system.ViewModels.AdminMainPage
{
    public partial class AdminMainPageViewModel : ViewModelBase
    {

        private MainWindowViewModel viewchange;

        private bool _isSidebarVisible = false; // Initialize as visible.

        public AdminMainPageViewModel(MainWindowViewModel mv)
        {
            viewchange = mv;
        }


        [RelayCommand]
        private void GoToUser()
        {
            viewchange.CurrentContent = new UserView() { DataContext = new UserMainPageViewModel(viewchange) };
        }

        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                if (_isSidebarVisible != value)
                {
                    _isSidebarVisible = value;
                    OnPropertyChanged(); // This calls the OnPropertyChanged method with the caller property name.
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
