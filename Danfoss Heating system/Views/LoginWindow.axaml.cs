using Avalonia.Controls;
using Danfoss_Heating_system.ViewModels;
using System;

namespace Danfoss_Heating_system.Views;

public partial class LoginWindow : Window
{

    public LoginWindow()
    {
        var viewModel = new LoginWindowViewModel();
        DataContext = viewModel;
        viewModel.LoginWindow = this;
        InitializeComponent();
        
    }

}