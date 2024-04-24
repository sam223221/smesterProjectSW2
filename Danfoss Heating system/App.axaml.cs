using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Danfoss_Heating_system.ViewModels;
using Danfoss_Heating_system.Views;

namespace Danfoss_Heating_system;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginWindow = new MainWindow();
            loginWindow.DataContext = new MainWindowViewModel("Admin",loginWindow); // Passes the window so it can be manipulated
            desktop.MainWindow = loginWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}