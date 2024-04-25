using Avalonia.Controls;
using Danfoss_Heating_system.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Danfoss_Heating_system.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
      
    }
}
