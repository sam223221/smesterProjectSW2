using CommunityToolkit.Mvvm.ComponentModel;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Danfoss_Heating_system.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{



    [ObservableProperty]
    private string password="Password";

    [ObservableProperty]
    private string username = "Username";

    [ObservableProperty]
    private string selectedQuote;

    [ObservableProperty]
    public string selectedQuoteAuthor;

    [ObservableProperty]
    private bool warningSign = false;

    [RelayCommand]
    private void WrongUsernameOrPassword()
    {
        WarningSign = true;
    }


    public MainWindowViewModel()
    {

        var parser = new ExcelDataParser("Assets/data.xlsx");
        var quotes  = parser.ParseQuotes();     // extracting the quotes from the excel sheet

        // Select a random quote
        Random random = new();
        var randomQuote = quotes[random.Next(quotes.Count)];
        selectedQuote = randomQuote.DisplayQuotes;
        selectedQuoteAuthor = randomQuote.DisplayQuoteAuthor;

        

 
    }



}
