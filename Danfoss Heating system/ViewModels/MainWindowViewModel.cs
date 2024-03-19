using Danfoss_Heating_system.Models;
using System.Collections.ObjectModel;

namespace Danfoss_Heating_system.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public ObservableCollection<EnergyData> WinterData { get; set; } = new ObservableCollection<EnergyData>();
    public ObservableCollection<EnergyData> SummerData { get; set; } = new ObservableCollection<EnergyData>();

    public MainWindowViewModel()
    {
        var parser = new ExcelDataParser();
        var data = parser.ParseExcel("Assets/data.xlsx");

        foreach (var item in data)
        {
            if (item.Season == "Winter")
                WinterData.Add(item);
            else
                SummerData.Add(item);
        }
    }


}
