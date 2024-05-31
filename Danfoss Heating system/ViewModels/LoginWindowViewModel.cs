using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using Danfoss_Heating_system.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Danfoss_Heating_system.ViewModels;

public partial class LoginWindowViewModel : ViewModelBase
{

    private Window closeable;

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string username = "";

    [ObservableProperty]
    private string selectedQuote;

    [ObservableProperty]
    public string selectedQuoteAuthor;

    [ObservableProperty]
    private bool warningSign = false;

    [ObservableProperty]
    private bool signInSucceed = false;

    ExcelDataParser excelDataParser = new ExcelDataParser("Assets/data.xlsx");
    EnergyData UserLogin;

    //Create Main window and sets the data context
    Window mainWindow = new MainWindow();
    MainWindowViewModel mainWindowViewModel;

    [RelayCommand]
    private void WrongUsernameOrPassword()
    {
        List<EnergyData> UserData = excelDataParser.UserInfo();

        foreach (var item in UserData)
        {

            // checks if the Login was successful
            if (item.UserID == Username)
            {
                if (item.UserPassword == Password)
                {
                    SignInSucceed = true;
                    UserLogin = item;
                    Debug.WriteLine("Login successful");
                    MainWindowOpen(item);
                    return;
                }
            }
        }
        WarningSign = true;
    }

    public void MainWindowOpen(EnergyData item)
    {
        mainWindowViewModel = new MainWindowViewModel(item, mainWindow);
        mainWindow.DataContext = mainWindowViewModel;

        //Closes and opens login page and main window
        mainWindow.Show();
        closeable.Close();
    }


    public LoginWindowViewModel(Window Cloneable)
    {
        closeable = Cloneable;
        var parser = new ExcelDataParser("Assets/data.xlsx");
        var quotes = parser.ParseQuotes();     // extracting the quotes from the excel sheet


        // Select a random quote and display it
        Random random = new();

        var randomQuote = quotes[random.Next(quotes.Count)];
        selectedQuote = randomQuote.DisplayQuotes;
        selectedQuoteAuthor = randomQuote.DisplayQuoteAuthor;

    }


}
