using CommunityToolkit.Mvvm.ComponentModel;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office2010.CustomUI;

namespace Danfoss_Heating_system.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    [ObservableProperty]
    private string password="";

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

    [RelayCommand]
    private void WrongUsernameOrPassword()
    {
        var parser = new ExcelDataParser("Assets/data.xlsx");
        var UserData = parser.UserInfo();
        EnergyData UserLogin;
        
        foreach (var item in UserData)
        {
            Console.WriteLine(item);

            // checks if the Login was successful
            if (item.UserID == Username)
            {
                if (item.UserPassword == Password)
                {
                    UserLogin = item;
                    Debug.WriteLine("Login successful");
                    SignInSucceed = true;
                    WarningSign = false;
                    return;
                }
            }
        }
        SignInSucceed = false;
        WarningSign = true;
    }


    public MainWindowViewModel()
    {

        var parser = new ExcelDataParser("Assets/data.xlsx");
        var quotes  = parser.ParseQuotes();     // extracting the quotes from the excel sheet
        

        // Select a random quote and display it
        Random random = new();

        var randomQuote     = quotes[random.Next(quotes.Count)];
        selectedQuote       = randomQuote.DisplayQuotes;
        selectedQuoteAuthor = randomQuote.DisplayQuoteAuthor;

    }



}
