using Avalonia.Controls;
using Danfoss_Heating_system.Interfaces;

namespace Danfoss_Heating_system.Views;

public partial class LoginWindow : Window, ICloseble
{

    public LoginWindow()
    {
        InitializeComponent();

    }

    private void TextBlock_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        InfoPupup.IsVisible = !InfoPupup.IsVisible;

    }

}