using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Drawing;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class GraphOptimiserViewModel : ViewModelBase
{



    // ----------- Properties for Bindings ----------- //
    [ObservableProperty]
    private string winterButtonForeground = "Green";
    [ObservableProperty]
    private string winterButtonBackground = "LightGreen";
    [ObservableProperty]
    private int selectedIndex;
    
    [ObservableProperty]
    private string summerButtonForeground = "Red";
    [ObservableProperty]
    private string summerButtonBackground = "LightCoral";


    
    
    // ----------- List for variables ----------- //

    List<EnergyData> allData;

    private MainWindowViewModel viewChange;

    private SortingTheDaysInExcelParser summerData = new SortingTheDaysInExcelParser();
    private SortingTheDaysInExcelParser winterData = new SortingTheDaysInExcelParser();

    private Dictionary<DateTime, List<EnergyData>> summerPeriodData;
    private Dictionary<DateTime, List<EnergyData>> winterPeriodData;

    private List<EnergyData> summerPeriod = new List<EnergyData>();
    private List<EnergyData> winterPeriod = new List<EnergyData>();




    // Constructor
    public GraphOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;

        viewChange.window.CanResize = true;

        viewChange.window.Width = 1980;
        viewChange.window.Height = 1020;

        allData = new ExcelDataParser("Assets/data.xlsx").ParserEnergyData();

        WinterPeriodDataConstructor();
    }


    // ----------- RelayCommands for buttons ----------- //

    [RelayCommand]
    private void WinterButton()
    {
        WinterButtonForeground = "Green";
        WinterButtonBackground = "LightGreen";
        SummerButtonForeground = "Red";
        SummerButtonBackground = "LightCoral";

        WinterPeriodDataConstructor();
    }

    [RelayCommand]
    private void SummerButton()
    {
        WinterButtonForeground = "Red";
        WinterButtonBackground = "LightCoral";
        SummerButtonForeground = "Green";
        SummerButtonBackground = "LightGreen";
        
        SummerPeriodDataConstructor();
    }



    public ObservableCollection<SortingTheDaysInExcelParserProps> DataDisplayed { get; set; } = new();

    // ----------- section for Methods ----------- // 
    private void WinterPeriodDataConstructor()
    {
        DataDisplayed.Clear();
        
        foreach (var item in allData)
        {
            if (item.Season == "Winter")
            {
                winterPeriod.Add(item);
            }
        }

        winterPeriodData = winterData.SortDataByDate(winterPeriod);
        foreach (var item in winterPeriodData)
        {
            DataDisplayed.Insert(0, new SortingTheDaysInExcelParserProps { date = item.Key.Date.ToString(), data = item.Value });
        }

    }

    private void SummerPeriodDataConstructor()
    {
        DataDisplayed.Clear();

        foreach (var item in allData)
        {
            if (item.Season == "Summer")
            {
                summerPeriod.Add(item);
                summerPeriodData = summerData.SortDataByDate(summerPeriod);
            }
        }
        foreach (var item in summerPeriodData)
        {
            DataDisplayed.Insert(0,new SortingTheDaysInExcelParserProps { date = item.Key.ToString(), data = item.Value });
        }

    }
}

