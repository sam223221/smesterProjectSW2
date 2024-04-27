using CommunityToolkit.Mvvm.ComponentModel;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class GraphOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;

    [ObservableProperty]
    private string winterButtonForeground = "Gray";

    public GraphOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;

        viewChange.window.Width = 1980;
        viewChange.window.Height = 1080;

        var paerser = new ExcelDataParser("/Assets/data.xlsx");
        var allData = paerser.ParserEnergyData();

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

            var summerData = new SortingTheDaysInExcelParser().SortDataByDate(summerPeriod);
            var winterData = new SortingTheDaysInExcelParser().SortDataByDate(winterPeriod);
        }
    }


}

