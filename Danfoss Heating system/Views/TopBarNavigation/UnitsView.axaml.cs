using Avalonia.Controls;
using Danfoss_Heating_system.ViewModels.TopBarNavigation;

namespace Danfoss_Heating_system.Views.TopBarNavigation
{
    public partial class UnitsView : UserControl
    {
        public UnitsView()
        {
            InitializeComponent();
            this.DataContext = new UnitsViewModel();
        }  
    }
}
