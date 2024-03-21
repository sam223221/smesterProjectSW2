using Avalonia.Controls;
using Danfoss_Heating_system.ViewModels;
using System;

namespace Danfoss_Heating_system.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void Binding(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
    }
}