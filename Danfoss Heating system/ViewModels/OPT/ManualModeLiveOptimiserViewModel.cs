using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using System;
using System.Collections.Generic;

namespace Danfoss_Heating_system.ViewModels.OPT
{
    public partial class ManualModeLiveOptimiserViewModel : ViewModelBase
    {
        private OPTLive optLive;

        private MainWindowViewModel viewChange;
        [ObservableProperty]
        private int sideBarWidth = 300;
        [ObservableProperty]
        private bool sideBarOpen = true;

        [ObservableProperty]
        private string currentProduction;

        // var for the heat demand and prediction
        [ObservableProperty]
        private string heatDemandCurrent;
        [ObservableProperty]
        private double heatDemandPrediction;

        // production cost and CO2 emissions for side bar
        [ObservableProperty]
        private string productionCost;
        [ObservableProperty]
        private string cO2Emotions;


        // color states of the boilers and motors
        [ObservableProperty]
        private string gasBoilerState = "Red";
        [ObservableProperty]
        private string oilBoilerState = "Red";
        [ObservableProperty]
        private string gasMotorState = "Red";
        [ObservableProperty]
        private string electricBoilerState = "Red";


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
        private int gasBoilerOperationPercent = 0;
        [ObservableProperty]
        private string gasBoilerOperationCost = "0";
        [ObservableProperty]
        private string gasBoilerOperationCO2 = "0";
        [ObservableProperty]
        private string gasBoilerOperation = "OFF";
        [ObservableProperty]
        private string gasBoilerUsingHeatDemand = "0";
        [ObservableProperty]
        private bool gasBoilerSettings = false;

        [ObservableProperty]
        private int oilBoilerOperationPercent = 0;
        [ObservableProperty]
        private string oilBoilerOperationCost = "0";
        [ObservableProperty]
        private string oilBoilerOperationCO2 = "0";
        [ObservableProperty]
        private string oilBoilerOperation = "OFF";
        [ObservableProperty]
        private string oilBoilerUsingHeatDemand = "0";
        [ObservableProperty]
        private bool oilBoilerSettings = false;

        [ObservableProperty]
        private int gasMotorOperationPercent = 0;
        [ObservableProperty]
        private string gasMotorOperationCost = "0";
        [ObservableProperty]
        private string gasMotorOperationCO2 = "0";
        [ObservableProperty]
        private string gasMotorOperation = "OFF";
        [ObservableProperty]
        private string gasMotorUsingHeatDemand = "0";
        [ObservableProperty]
        private bool gasMotorSettings = false;

        [ObservableProperty]
        private int electricBoilerOperationPercent = 0;
        [ObservableProperty]
        private string electricBoilerOperationCost = "0";
        [ObservableProperty]
        private string electricBoilerOperationCO2 = "0";
        [ObservableProperty]
        private string electricBoilerOperation = "OFF";
        [ObservableProperty]
        private string electricBoilerUsingHeatDemand = "0";
        [ObservableProperty]
        private bool electricBoilerSettings = false;

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
        private bool autoModeSwitch = false;


        [ObservableProperty]
        private string actualHour;

        [ObservableProperty]
        private string previousHour;

        [ObservableProperty]
        private string nextHour;


        [ObservableProperty]
        private bool changesAreBeingMade = false;

        private bool seasonSelecter = true;





        private OPTLive OPTLive;
        List<OPTLiveProp> unitState = new();

        public ManualModeLiveOptimiserViewModel(MainWindowViewModel viewChange)
        {
            this.viewChange = viewChange;
            optLive = new OPTLive("/Assets/data.xlsx");
            Initilizer();
        }


        // relay commands for the summer and winter buttons
        [RelayCommand]
        private void SummerTrue()
        {
            seasonSelecter = false;
            SummerFontWeight = "Bold";
            SummerBackground = "Green";
            WinterFontWeight = "Normal";
            WinterBackground = "Gray";
            Initilizer();

        }
        [RelayCommand]
        private void WinterTrue()
        {
            seasonSelecter = true;
            SummerFontWeight = "Normal";
            SummerBackground = "Gray";
            WinterFontWeight = "Bold";
            WinterBackground = "Green";
            Initilizer();
        }


        [RelayCommand]
        private void OnOfButtonForMachins(string name)
        {
            ChangesAreBeingMade = true;
            foreach (var unit in unitState)
            {
                if (unit.NameOfUnit == name && unit.isUnitEnabled == false)
                {
                    unit.UsageInPercentPerHour = 100;
                    unit.stateOfUnit = "Green";
                    unit.operationOfUnit = "ON";
                    unit.operationCost = unit.data.MaxHeat * unit.data.ProductionCost;
                    unit.usingCO2Emission = unit.data.MaxHeat * unit.data.CO2Emission;
                    unit.usingHeatDemand = unit.data.MaxHeat;
                    unit.isUnitEnabled = true;
                    UnitUpdate(unit);
                }
                else if (unit.NameOfUnit == name && unit.isUnitEnabled == true)
                {
                    unit.UsageInPercentPerHour = 0;
                    unit.stateOfUnit = "Red";
                    unit.operationOfUnit = "OFF";
                    unit.operationCost = 0;
                    unit.usingCO2Emission = 0;
                    unit.usingHeatDemand = 0;
                    unit.isUnitEnabled = false;
                    UnitUpdate(unit);
                }
            }
        }


        [RelayCommand]
        private void SettingsIsVisible(string unitName)
        {
            switch (unitName)
            {
                case "Gas boiler":
                    GasBoilerSettings = !GasBoilerSettings;
                    break;
                case "Oil boiler":
                    OilBoilerSettings = !OilBoilerSettings;
                    break;
                case "Gas motor":
                    GasMotorSettings = !GasMotorSettings;
                    break;
                case "Electric boiler":
                    ElectricBoilerSettings = !ElectricBoilerSettings;
                    break;
            };
        }


        [RelayCommand]
        private void MinusHeatDemand(string name)
        {
            ChangesAreBeingMade = true;
            foreach (var unit in unitState)
            {
                if (unit.NameOfUnit == name && unit.isUnitEnabled == true)
                {
                    unit.UsageInPercentPerHour -= 5;
                    unit.usingHeatDemand -= Math.Round(unit.data.MaxHeat / 20, 3);

                    if (unit.UsageInPercentPerHour < 5)
                    {
                        unit.stateOfUnit = "Red";
                        unit.isUnitEnabled = false;
                    }
                    else
                    {
                        unit.isUnitEnabled = true;
                    }
                    UnitUpdate(unit);
                }
            }
        }

        [RelayCommand]
        private void PlusHeatDemand(string name)
        {
            ChangesAreBeingMade = true;
            foreach (var unit in unitState)
            {
                if (unit.NameOfUnit == name && unit.UsageInPercentPerHour < 100)
                {
                    unit.stateOfUnit = "Green";
                    unit.operationOfUnit = "ON";
                    unit.UsageInPercentPerHour += 5;
                    unit.usingHeatDemand += Math.Round(unit.data.MaxHeat / 20, 3);
                    unit.isUnitEnabled = true;
                    UnitUpdate(unit);
                }
            }
        }

        private void UnitUpdate(OPTLiveProp unit)
        {
            switch (unit.NameOfUnit)
            {
                case "Gas boiler":
                    GasBoilerOperationPercent = unit.UsageInPercentPerHour;
                    GasBoilerOperationCost = unit.operationCost.ToString("F2");
                    GasBoilerOperationCO2 = unit.usingCO2Emission.ToString("F2");
                    GasBoilerUsingHeatDemand = unit.usingHeatDemand.ToString("F2");
                    GasBoilerState = unit.stateOfUnit;
                    GasBoilerOperation = unit.operationOfUnit;
                    break;
                case "Oil boiler":
                    OilBoilerOperationPercent = unit.UsageInPercentPerHour;
                    OilBoilerOperationCost = unit.operationCost.ToString("F2");
                    OilBoilerOperationCO2 = unit.usingCO2Emission.ToString("F2");
                    OilBoilerUsingHeatDemand = unit.usingHeatDemand.ToString("F2");
                    OilBoilerState = unit.stateOfUnit;
                    OilBoilerOperation = unit.operationOfUnit;
                    break;
                case "Gas motor":
                    GasMotorOperationPercent = unit.UsageInPercentPerHour;
                    GasMotorOperationCost = unit.operationCost.ToString("F2");
                    GasMotorOperationCO2 = unit.usingCO2Emission.ToString("F2");
                    GasMotorUsingHeatDemand = unit.usingHeatDemand.ToString("F2");
                    GasMotorState = unit.stateOfUnit;
                    GasMotorOperation = unit.operationOfUnit;
                    break;
                case "Electric boiler":
                    ElectricBoilerOperationPercent = unit.UsageInPercentPerHour;
                    ElectricBoilerOperationCost = unit.operationCost.ToString("F2");
                    ElectricBoilerOperationCO2 = unit.usingCO2Emission.ToString("F2");
                    ElectricBoilerUsingHeatDemand = unit.usingHeatDemand.ToString("F2");
                    ElectricBoilerState = unit.stateOfUnit;
                    ElectricBoilerOperation = unit.operationOfUnit;
                    break;
            }
            ProductionCost = optLive.ProductionCostPerHourManualMode(unitState).ToString("F2");
            CO2Emotions = optLive.CO2EmmitionsPerHourManualMode(unitState).ToString("F2");
            CurrentProduction = optLive.TotalHeatProductionManualMode(unitState).ToString("F2");
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
        private void ChangeSettings(string chose)
        {
            if (chose == "Apply")
            {
                ChangesAreBeingMade = false;
                optLive.StoreUnitState(unitState, seasonSelecter);
            }
            else if (chose == "Cancel")
            {
                ChangesAreBeingMade = false;
                Initilizer();
            }
        }

        [RelayCommand]
        private void AutoMode(string? answer)
        {
            AutoModeSwitch = !AutoModeSwitch;

            if (answer == "yes")
            {
                viewChange.CurrentContent = new LiveOptimiser() { DataContext = new LiveOptimiserViewModel(viewChange) };
                return;
            }

        }

        public void Initilizer()
        {
            // Units initialization
            unitState = optLive.MachinesInitialize(optLive.HourInformation(seasonSelecter), seasonSelecter);

            foreach (var item in unitState)
            {
                UnitUpdate(item);
            }

            // Side bar initialization
            ProductionCost = optLive.ProductionCostPerHourManualMode(unitState).ToString("F2");
            CO2Emotions = optLive.CO2EmmitionsPerHourManualMode(unitState).ToString("F2");

            // Heat demand and prediction initialization
            CurrentProduction = optLive.TotalHeatProductionManualMode(unitState).ToString("F2");
            HeatDemandCurrent = optLive.HourInformation(seasonSelecter).HeatDemand.ToString("F2");
            HeatDemandPrediction = optLive.predictHeatDemandCalculate(seasonSelecter);

            // Clock initialization
            PreviousHour = optLive.GetHourFromCellIndex(0, seasonSelecter);
            ActualHour = optLive.GetHourFromCellIndex(1, seasonSelecter);
            NextHour = optLive.GetHourFromCellIndex(2, seasonSelecter);

        }
    }
}
