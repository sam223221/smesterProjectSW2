using Avalonia.Controls;
using Danfoss_Heating_system.ViewModels;
using System;

namespace Danfoss_Heating_system.Views;

public partial class LoginWindow : Window
{

    public LoginWindow()
    {
        InitializeComponent();

        LoginWindowViewModel viewModel = new LoginWindowViewModel();
        viewModel.LoginWindow = this;
    }

}