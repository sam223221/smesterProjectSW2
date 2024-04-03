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
        private ViewModelBase contentViewModel;

        [ObservableProperty]
        private string role ="bob what is this";

        public MainWindowViewModel(string roles) 
        {
            Role = roles;           
        }
    }
}