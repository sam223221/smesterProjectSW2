using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using System.Linq;


namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class LiveOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;
    private OPTLive OPTLive;
    private OPTLiveProp OPTLiveProp;

    [ObservableProperty]
    private int _sideBarWidth = 0;
    [ObservableProperty]
    private bool sideBarOpen = false;

    // var for the heat demand and prediction
    [ObservableProperty]
    private double heatDemandCurrent;
    [ObservableProperty]
    private double heatDemandPrediction;

    // production cost and CO2 emissions for side bar
    [ObservableProperty]
    private int productionCost;
    [ObservableProperty]
    private int cO2Emotions;


    // color states of the boilers and motors
    [ObservableProperty]
    private string gasBoilerState;
    [ObservableProperty]
    private string oilBoilerState;
    [ObservableProperty]
    private string gasMotorState;
    [ObservableProperty]
    private string electricBoilerState;


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
    private int gasBoilerOperationPercent;
    [ObservableProperty]
    private int gasBoilerOperationCost;
    [ObservableProperty]
    private int gasBoilerOperationCO2;
    [ObservableProperty]
    private string gasBoilerOperation;

    [ObservableProperty]
    private int oilBoilerOperationPercent;
    [ObservableProperty]
    private int oilBoilerOperationCost;
    [ObservableProperty]
    private int oilBoilerOperationCO2;
    [ObservableProperty]
    private string oilBoilerOperation;

    [ObservableProperty]
    private int gasMotorOperationPercent;
    [ObservableProperty]
    private int gasMotorOperationCost;
    [ObservableProperty]
    private int gasMotorOperationCO2;
    [ObservableProperty]
    private string gasMotorOperation;

    [ObservableProperty]
    private int electricBoilerOperationPercent;
    [ObservableProperty]
    private int electricBoilerOperationCost;
    [ObservableProperty]
    private int electricBoilerOperationCO2;
    [ObservableProperty]
    private string electricBoilerOperation;

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


    [ObservableProperty]
    private string actualHour;

    [ObservableProperty]
    private string previousHour;

    [ObservableProperty]
    private string nextHour;


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
            SettingsForDynamicData();
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
        ECOFriendly = !ECOFriendly;

        if (ECOFriendly)
        {
            SettingsForDynamicData();
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
            viewChange.CurrentContent = new ManualModeLiveOptimiserView() { DataContext = new ManualModeLiveOptimiserViewModel(viewChange) };
            return;
        }
    }

    // constructor for the live optimiser view model
    public LiveOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;
        viewChange.window.CanResize = true;
        viewChange.window.Width = 1920;
        viewChange.window.Height = 1080;
        OPTLive = new OPTLive("/Assets/data.xlsx");
        SettingsForDynamicData();
    }

    public void SettingsForDynamicData()
    {
        OPTLive.UsingMachines(160, true);
        ProductionCost = (int)OPTLive.ProductionCostPerHour();
        CO2Emotions = (int)OPTLive.CO2EmmitionsPerHour();

        var usagePerHour = OPTLive.UnitUsagePerHour();
        GasBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
        OilBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
        GasMotorOperationPercent = usagePerHour.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
        ElectricBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();

        HeatDemandCurrent = OPTLive.TotalHeatDemand();
        HeatDemandPrediction = OPTLive.predictHeatDemandCalculate(160, true, "Assets/data.xlsx");
        PreviousHour = OPTLive.GetHourFromCellIndex(159, true, "Assets/data.xlsx");
        ActualHour = OPTLive.GetHourFromCellIndex(160, true, "Assets/data.xlsx");
        NextHour = OPTLive.GetHourFromCellIndex(161, true, "Assets/data.xlsx");
        var operationcost = OPTLive.UnitsOperationCosts();
        GasBoilerOperationCost = operationcost.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
        OilBoilerOperationCost = operationcost.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
        GasMotorOperationCost = operationcost.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
        ElectricBoilerOperationCost = operationcost.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();


        var co2emissions = OPTLive.UnitsCO2Emission();
        GasBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
        OilBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
        GasMotorOperationCO2 = co2emissions.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
        ElectricBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();


        var stateofUnit = OPTLive.StateOfUnits(160, true, "Assets/data.xlsx");

        stateofUnit.TryGetValue("Gas boiler", out var gasBoilerInfo);
        GasBoilerState = gasBoilerInfo.Item1;
        GasBoilerOperation = gasBoilerInfo.Item2;

        stateofUnit.TryGetValue("Oil boiler", out var oilBoilerInfo);
        OilBoilerState = oilBoilerInfo.Item1;
        OilBoilerOperation = oilBoilerInfo.Item2;

        stateofUnit.TryGetValue("Gas motor", out var gasMotorInfo);
        GasMotorState = gasMotorInfo.Item1;
        GasMotorOperation = gasMotorInfo.Item2;

        stateofUnit.TryGetValue("Electric boiler", out var electricBoilerInfo);
        ElectricBoilerState = electricBoilerInfo.Item1;
        ElectricBoilerOperation = electricBoilerInfo.Item2;
    }
}
