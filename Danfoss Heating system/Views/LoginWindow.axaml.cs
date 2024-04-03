using Avalonia.Controls;
using Danfoss_Heating_system.Interfaces;
using Danfoss_Heating_system.ViewModels;
using System;

namespace Danfoss_Heating_system.Views;

public partial class LoginWindow : Window , ICloseble
{

    public LoginWindow()
    {
        InitializeComponent();
        
    }

    public void Close()
    {
        base.Close();
    }

}