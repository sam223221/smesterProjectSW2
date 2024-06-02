using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;


namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class LiveOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;
    private OPTLive OPTLive;

    [ObservableProperty]
    private int _sideBarWidth = 0;
    [ObservableProperty]
    private bool sideBarOpen = false;

    // var for the heat demand and prediction
    [ObservableProperty]
    private string heatDemandCurrent;
    [ObservableProperty]
    private double heatDemandPrediction;
    [ObservableProperty]
    private string heatDemandproduction;


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

    private string scenario = "BestCost";

    //manual Mode
    [ObservableProperty]
    private bool manualModeVisible = false;

    private bool seasonSelected = true;

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
        seasonSelected = false;
        SettingsForDynamicData();
        SummerFontWeight = "Bold";
        SummerBackground = "Green";
        WinterFontWeight = "Normal";
        WinterBackground = "Gray";
    }
    [RelayCommand]
    private void WinterTrue()
    {
        seasonSelected = true;
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

        scenario = "BestCost";
        SettingsForDynamicData();
        LowCostWeight = "Bold";
        LowCostBackground = "Green";
        ECOFriendlyFontWeight = "Normal";
        ECOFriendlyBackground = "Gray";
    }

    [RelayCommand]
    private void ECOFriendlyTrue()
    {
        scenario = "LowestCO2";
        SettingsForDynamicData();
        ECOFriendlyFontWeight = "Bold";
        ECOFriendlyBackground = "Green";
        LowCostWeight = "Normal";
        LowCostBackground = "Gray";
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

        var Units = OPTLive.UsingMachines(seasonSelected, scenario);

        foreach (var unit in Units)
        {
            switch (unit.NameOfUnit)
            {
                case "Gas boiler":
                    GasBoilerOperationPercent = unit.UsageInPercentPerHour;
                    GasBoilerOperationCost = (int)unit.operationCost;
                    GasBoilerOperationCO2 = (int)unit.usingCO2Emission;
                    GasBoilerState = unit.stateOfUnit;
                    GasBoilerOperation = unit.operationOfUnit;
                    break;
                case "Oil boiler":
                    OilBoilerOperationPercent = unit.UsageInPercentPerHour;
                    OilBoilerOperationCost = (int)unit.operationCost;
                    OilBoilerOperationCO2 = (int)unit.usingCO2Emission;
                    OilBoilerState = unit.stateOfUnit;
                    OilBoilerOperation = unit.operationOfUnit;
                    break;
                case "Gas motor":
                    GasMotorOperationPercent = unit.UsageInPercentPerHour;
                    GasMotorOperationCost = (int)unit.operationCost;
                    GasMotorOperationCO2 = (int)unit.usingCO2Emission;
                    GasMotorState = unit.stateOfUnit;
                    GasMotorOperation = unit.operationOfUnit;
                    break;
                case "Electric boiler":
                    ElectricBoilerOperationPercent = unit.UsageInPercentPerHour;
                    ElectricBoilerOperationCost = (int)unit.operationCost;
                    ElectricBoilerOperationCO2 = (int)unit.usingCO2Emission;
                    ElectricBoilerState = unit.stateOfUnit;
                    ElectricBoilerOperation = unit.operationOfUnit;
                    break;
            }
        }
        HeatDemandproduction = OPTLive.productionCost.ToString("F2");
        HeatDemandCurrent = OPTLive.currentHeatDemand.ToString("F2");
        HeatDemandPrediction = OPTLive.predictHeatDemandCalculate(seasonSelected);

        PreviousHour = OPTLive.GetHourFromCellIndex(0, seasonSelected);
        ActualHour = OPTLive.GetHourFromCellIndex(1, seasonSelected);
        NextHour = OPTLive.GetHourFromCellIndex(2, seasonSelected);



        OPTLive.UsingMachines(seasonSelected, scenario);
        ProductionCost = (int)OPTLive.productionCost;
        CO2Emotions = (int)OPTLive.CO2Emission;


    }
}
