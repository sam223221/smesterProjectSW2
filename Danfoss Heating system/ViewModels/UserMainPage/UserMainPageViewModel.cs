using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.ViewModels.AdminMainPage;

namespace Danfoss_Heating_system.ViewModels.UserMainPage
{
    public partial class UserMainPageViewModel : ViewModelBase
    {

        private MainWindowViewModel viewchange;

        public UserMainPageViewModel(MainWindowViewModel mv)
        {
            viewchange = mv;
        }

        [RelayCommand]
        private void AdminView()
        {
            viewchange.CurrentContent = new AdminView() { DataContext = new AdminMainPageViewModel(viewchange) };
        }
    }
}