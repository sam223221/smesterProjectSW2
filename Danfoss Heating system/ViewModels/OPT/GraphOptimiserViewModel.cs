using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class GraphOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;
    private SortingTheDaysInExcelParser summerData = new SortingTheDaysInExcelParser();
    private SortingTheDaysInExcelParser winterData = new SortingTheDaysInExcelParser();

    private Dictionary<DateTime, List<EnergyData>> summerPeriod;
    private Dictionary<DateTime, List<EnergyData>> winterPeriod;

    [ObservableProperty]
    private string winterButtonForeground = "Green";
    [ObservableProperty]
    private string winterButtonBackground = "LightGreen";
    
    [ObservableProperty]
    private string summerButtonForeground = "Red";
    [ObservableProperty]
    private string summerButtonBackground = "LightCoral";

    ObservableCollection<SortingTheDaysInExcelParserProps> DataDisplayed;

    public GraphOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;

        viewChange.window.CanResize = true;

        viewChange.window.Width = 1980;
        viewChange.window.Height = 1020;

        var parser = new ExcelDataParser("Assets/data.xlsx");
        var allData = parser.ParserEnergyData();

        var summerPeriod = new List<EnergyData>();
        var winterPeriod = new List<EnergyData>();

        foreach (var item in allData)
        {
            if (item.Season == "Summer")
            {
                summerPeriod.Add(item);
            }
            else if (item.Season == "Winter")
            {
                winterPeriod.Add(item);
            }

            this.summerPeriod = summerData.SortDataByDate(summerPeriod);
            this.winterPeriod = winterData.SortDataByDate(winterPeriod);
        }
    }

    [RelayCommand]
    private void WinterButton()
    {
        WinterButtonForeground = "Green";
        WinterButtonBackground = "LightGreen";
        SummerButtonForeground = "Red";
        SummerButtonBackground = "LightCoral";

        DataDisplayed.Clear();

        foreach (var item in winterPeriod)
        {
            DataDisplayed.Add(new SortingTheDaysInExcelParserProps { date = item.Key.ToString(), data = item.Value });
        }

    }

    [RelayCommand]
    private void SummerButton()
    {
        WinterButtonForeground = "Red";
        WinterButtonBackground = "LightCoral";
        SummerButtonForeground = "Green";
        SummerButtonBackground = "LightGreen";
    }


}

