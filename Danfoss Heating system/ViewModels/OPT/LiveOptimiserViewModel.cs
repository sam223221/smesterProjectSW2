using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Reactive;
using System.Security.AccessControl;


namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class LiveOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;
    private OPTLive OPTLive;
    private OPTLiveProp OPTLiveProp;

    [ObservableProperty]
    private int _sideBarWidth = 300;
    [ObservableProperty]
    private bool sideBarOpen = true;

    // var for the heat demand and prediction
    [ObservableProperty]
    private double heatDemandCurrent = 6.7;
    [ObservableProperty]
    private double heatDemandPrediction = 7.2;

    // production cost and CO2 emissions for side bar
    [ObservableProperty]
    private int productionCost = 800;
    [ObservableProperty]
    private int cO2Emotions = 850;


    // color states of the boilers and motors
    [ObservableProperty]
    private string gasBoilerState = "Green";
    [ObservableProperty]
    private string oilBoilerState = "Green";
    [ObservableProperty]
    private string gasMotorState = "Orange";
    [ObservableProperty]
    private string electricBoilerState = "Gray";


    // color states of the summer and winter buttons
    [ObservableProperty]
    private string summerFontWeight = "Normal";
    [ObservableProperty]
    private string summerBackground = "Gray";
    [ObservableProperty]
    private string winterFontWeight = "Bold";
    [ObservableProperty]
    private string winterBackground = "Green";


    // operation states of the boilers and motors
    [ObservableProperty]
    private int gasBoilerOperationPercent= 100;
    [ObservableProperty]
    private int gasBoilerOperationCost = 214;
    [ObservableProperty]
    private int gasBoilerOperationCO2 = 300;
    [ObservableProperty]
    private string gasBoilerOperation = "ON";

    [ObservableProperty]
    private int oilBoilerOperationPercent = 100;
    [ObservableProperty]
    private int oilBoilerOperationCost = 312;
    [ObservableProperty]
    private int oilBoilerOperationCO2 = 180;
    [ObservableProperty]
    private string oilBoilerOperation = "ON";

    [ObservableProperty]
    private int gasMotorOperationPercent = 12;
    [ObservableProperty]
    private int gasMotorOperationCost = 800;
    [ObservableProperty]
    private int gasMotorOperationCO2 = 20;
    [ObservableProperty]
    private string gasMotorOperation = "Heating up";

    [ObservableProperty]
    private int electricBoilerOperationPercent = 0;
    [ObservableProperty]
    private int electricBoilerOperationCost = 0;
    [ObservableProperty]
    private int electricBoilerOperationCO2 = 0;
    [ObservableProperty]
    private string electricBoilerOperation = "OFF";

    // color states of the low cost and eco friendly buttons
    [ObservableProperty]
    private string lowCostWeight = "Bold";
    [ObservableProperty]
    private string lowCostBackground = "Green";
    [ObservableProperty]
    private string eCOFriendlyFontWeight = "Normal";
    [ObservableProperty]
    private string eCOFriendlyBackground = "Gray";
    private bool LowCost = true;
    private bool ECOFriendly = false;

    //manual Mode
    [ObservableProperty]
    private bool manualModeVisible = false;



    // relay commands for the summer and winter buttons
    [RelayCommand]
    private void SummerTrue()
    {
        SettingsForDynamicData();
        SummerFontWeight = "Bold";
        SummerBackground = "Green";
        WinterFontWeight = "Normal";
        WinterBackground = "Gray";
    }
    [RelayCommand]
    private void WinterTrue()
    {
        SettingsForDynamicData();
        SummerFontWeight = "Normal";
        SummerBackground = "Gray";
        WinterFontWeight = "Bold";
        WinterBackground = "Green";
    }




    // relay commands for the low cost and eco friendly buttons
    [RelayCommand]
    private void LowCostTrue()
    {
        SettingsForDynamicData();
        LowCost = !LowCost;

        if (LowCost)
        {
            LowCostWeight = "Bold";
            LowCostBackground = "Green";
            return;
        }
        if (LowCost != true && ECOFriendly != false)
        {
            LowCostWeight = "Normal";
            LowCostBackground = "Gray";
            return;
        }
        else
        {
            LowCost = !LowCost;
            return;
        }
    }

    [RelayCommand]
    private void ECOFriendlyTrue()
    {
        SettingsForDynamicData();
        ECOFriendly = !ECOFriendly;

        if (ECOFriendly)
        {
            ECOFriendlyFontWeight = "Bold";
            ECOFriendlyBackground = "Green";
            return;
        }
        if (ECOFriendly != true && LowCost != false)
        {
            ECOFriendlyFontWeight = "Normal";
            ECOFriendlyBackground = "Gray";
            return;
        }
        else
        {
            ECOFriendly = !ECOFriendly;
            return;
        }

    }



    // relay command for the sidebar toggle
    [RelayCommand]
    private void SideBarToggle()
    {

        SideBarOpen = !SideBarOpen;
        if (SideBarOpen)
        {
            SideBarWidth = 300;
        }
        else
        {
            SideBarWidth = 0;
        }
    }

    [RelayCommand]
    private void ManualMode(string? answer)
    {
        ManualModeVisible = !ManualModeVisible;

        if (answer == "Yes")
        {
            viewChange.CurrentContent = new ManualModeLiveOptimiserView() { DataContext = new ManualModeLiveOptimiserViewModel(viewChange)};
            return;
        }
    }

    // constructor for the live optimiser view model
    public LiveOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;
        viewChange.window.CanResize = true;
        viewChange.window.Width = 1980;
        viewChange.window.Height = 1080;
        viewChange.window.MinHeight = 700;
        viewChange.window.MinWidth = 1800;
        OPTLive = new OPTLive("/Assets/data.xlsx");      
    }

    public void SettingsForDynamicData()
    {
        ProductionCost = (int)OPTLive.ProductionCostPerHour();
        CO2Emotions = (int)OPTLive.CO2EmmitionsPerHour();
        var usagePerHour = OPTLive.UnitUsagePerHour();
        GasBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
        OilBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
        GasMotorOperationPercent  = usagePerHour.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
        ElectricBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();
        HeatDemandCurrent = OPTLive.TotalHeatDemand();
        HeatDemandPrediction = OPTLiveProp.PredictedHeatDemand;
    }

}
