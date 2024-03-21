using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Danfoss_Heating_system.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{

    public ObservableCollection<EnergyData> WinterData { get; set; } = new ObservableCollection<EnergyData>();
    public ObservableCollection<EnergyData> SummerData { get; set; } = new ObservableCollection<EnergyData>();
    public string SelectedQuote { get; set; }
    public string SelectedQuoteAuthor { get; set; }


    public MainWindowViewModel()
    {

        var parser = new ExcelDataParser("Assets/data.xlsx");
        
        var quotes  = parser.ParseQuotes();     // extracting the quotes from the excel sheet
        var data    = parser.ParserEnergyData();// extracting the energy consumption on each date data

        Random random = new Random();



        // Select a random quote
        var randomQuote = quotes[random.Next(quotes.Count)];
        SelectedQuote = randomQuote.DisplayQuotes;
        SelectedQuoteAuthor = randomQuote.DisplayQuoteAuthor;

        

        foreach (var item in data)
        {
            if (item.Season == "Winter")
                WinterData.Add(item);
            else
                SummerData.Add(item);
        }
    }


}
