using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Danfoss_Heating_system.ViewModels.OPT;

namespace Danfoss_Heating_system.ViewModels.AdminMainPage
{
    public partial class AdminMainPageViewModel : ViewModelBase
    {

        private MainWindowViewModel viewchange;

        [ObservableProperty]
        private string userName;

        public AdminMainPageViewModel(MainWindowViewModel mv)
        {
            viewchange = mv;
            viewchange.window.Width = 800;
            viewchange.window.Height = 450;
            viewchange.window.CanResize = false;
            userName = "welcome back " + mv.userName.UserID;
        }


        [RelayCommand]
        private void GoToUser()
        {
            viewchange.CurrentContent = new GraphOptimiserView() { DataContext = new GraphOptimiserViewModel(viewchange) };
        }

        [RelayCommand]
        private void LiveOptimiser()
        {
            viewchange.CurrentContent = new LiveOptimiser() { DataContext = new LiveOptimiserViewModel(viewchange) };
        }

    }
}
