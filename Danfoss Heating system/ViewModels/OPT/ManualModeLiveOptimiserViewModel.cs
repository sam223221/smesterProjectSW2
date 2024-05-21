using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Danfoss_Heating_system.ViewModels.OPT
{
    public partial class ManualModeLiveOptimiserViewModel : ViewModelBase
    {
        private OPTLive optLive;

        private MainWindowViewModel viewChange;
        [ObservableProperty]
        private int _sideBarWidth = 300;
        [ObservableProperty]
        private bool sideBarOpen = true;

        [ObservableProperty]
        private double currentProduction;

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
        private double gasBoilerOperationCost = 0;
        [ObservableProperty]
        private double gasBoilerOperationCO2 = 0;
        [ObservableProperty]
        private string gasBoilerOperation = "OFF";
        [ObservableProperty]
        private double gasBoilerUsingHeatDemand = 0;

        [ObservableProperty]
        private int oilBoilerOperationPercent = 0;
        [ObservableProperty]
        private int oilBoilerOperationCost = 0;
        [ObservableProperty]
        private int oilBoilerOperationCO2 = 0;
        [ObservableProperty]
        private string oilBoilerOperation = "OFF";
        [ObservableProperty]
        private double oilBoilerUsingHeatDemand = 0;

        [ObservableProperty]
        private int gasMotorOperationPercent = 0;
        [ObservableProperty]
        private int gasMotorOperationCost = 0;
        [ObservableProperty]
        private int gasMotorOperationCO2 = 0;
        [ObservableProperty]
        private string gasMotorOperation = "OFF";
        [ObservableProperty]
        private double gasMotorUsingHeatDemand = 0;

        [ObservableProperty]
        private int electricBoilerOperationPercent = 0;
        [ObservableProperty]
        private int electricBoilerOperationCost = 0;
        [ObservableProperty]
        private int electricBoilerOperationCO2 = 0;
        [ObservableProperty]
        private string electricBoilerOperation = "OFF";
        [ObservableProperty]
        private double electricBoilerUsingHeatDemand = 0;

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
        private bool toolTipIsVisible;

        [RelayCommand]
        private void ToggleToolTipVisibility()
        {
            ToolTipIsVisible = !ToolTipIsVisible;
        }

        // relay commands for the summer and winter buttons
        [RelayCommand]
        private void SummerTrue()
        {

            SummerFontWeight = "Bold";
            SummerBackground = "Green";
            WinterFontWeight = "Normal";
            WinterBackground = "Gray";

        }
        [RelayCommand]
        private void WinterTrue()
        {

            SummerFontWeight = "Normal";
            SummerBackground = "Gray";
            WinterFontWeight = "Bold";
            WinterBackground = "Green";
        }

        public bool GasBoilerTurnOn;
        private bool OilBoilerTurnOn;
        private bool GasMotorTurnOn;
        private bool ElectricBoilerTurnOn;
       

        Dictionary<string, OPTLiveProp> newState = new Dictionary<string, OPTLiveProp>();
        public void HoldChangesNewList()
        {
            {
                foreach (var unit in optLive.MachinesManualMode(160, true))
                {
                    EnergyData energyData = new EnergyData
                    {
                        Name = unit.Value.data.Name,
                        HeatDemand = unit.Value.data.HeatDemand,
                        MaxHeat = unit.Value.data.MaxHeat,
                        MaxElectricity = unit.Value.data.MaxElectricity,
                        ProductionCost = unit.Value.data.ProductionCost,
                        CO2Emission = unit.Value.data.CO2Emission,
                        GasConsumption = unit.Value.data.GasConsumption,
                        PercentageUsage = unit.Value.data.PercentageUsage,
                        ElectricityPrice = unit.Value.data.ElectricityPrice,
                    };


                    OPTLiveProp optLivePropForManual = new OPTLiveProp(energyData)
                    {
                        isUnitEnabled = unit.Value.isUnitEnabled,
                        PredictedHeatDemand = unit.Value.PredictedHeatDemand,
                        remainingHeatDemand = unit.Value.remainingHeatDemand,
                        usingCO2Emission = unit.Value.usingHeatDemand * unit.Value.data.CO2Emission,
                       usingHeatDemand = unit.Value.usingHeatDemand,
                       stateOfUnit = unit.Value.stateOfUnit,
                       operationOfUnit = unit.Value.operationOfUnit,
                       UsageInPercentPerHour = unit.Value.UsageInPercentPerHour,
                    };

                    if (!newState.ContainsKey(unit.Value.data.Name))
                    {
                    newState.Add(unit.Value.data.Name, optLivePropForManual);
                    }

                    else
                    {
                        newState[unit.Value.data.Name] = optLivePropForManual;
                    }
                }
            }
         }

        private void UpdateListFromState(string name)
        {
                if (name == "Gas boiler" && newState.TryGetValue("Gas boiler", out OPTLiveProp gasBoiler))
                    {
                    gasBoiler.stateOfUnit = GasBoilerState;
                    gasBoiler.operationOfUnit = GasBoilerOperation;
                    gasBoiler.UsageInPercentPerHour = GasBoilerOperationPercent;
                    gasBoiler.operationCost = GasBoilerOperationCost;
                    gasBoiler.usingCO2Emission = GasBoilerOperationCO2;
                    if (gasBoiler.usingHeatDemand == 0)
                    {
                        gasBoiler.usingHeatDemand = GasBoilerUsingHeatDemand;
                    }
                   else if (gasBoiler.operationCost == 0)
                    {
                        gasBoiler.usingHeatDemand = 0;
                    }
                  

                    newState["Gas boiler"] = gasBoiler;
                    SettingsForDynamicDataManualMode();
                }
            
                else if (name == "Oil boiler" &&  newState.TryGetValue("Oil boiler", out OPTLiveProp oilBoiler))
                {
                    oilBoiler.stateOfUnit = OilBoilerState;
                    oilBoiler.operationOfUnit = OilBoilerOperation;
                    oilBoiler.UsageInPercentPerHour = OilBoilerOperationPercent;
                    oilBoiler.operationCost = OilBoilerOperationCost;
                    oilBoiler.usingCO2Emission = OilBoilerOperationCO2;
                    if(oilBoiler.usingHeatDemand == 0)
                    {
                        oilBoiler.usingHeatDemand = OilBoilerUsingHeatDemand;
                    }
                   else if (oilBoiler.operationCost == 0)
                    {
                        oilBoiler.usingHeatDemand = 0;
                    }

                newState["Oil boiler"] = oilBoiler;
                    SettingsForDynamicDataManualMode();
                }

               else if (name == "Gas motor" && newState.TryGetValue("Gas motor", out OPTLiveProp gasMotor))
               {
                    gasMotor.stateOfUnit = GasMotorState;
                    gasMotor.operationOfUnit = GasMotorOperation;
                    gasMotor.UsageInPercentPerHour = GasMotorOperationPercent;
                    gasMotor.operationCost = GasMotorOperationCost;
                    gasMotor.usingCO2Emission = GasMotorOperationCO2;

                  if (gasMotor.usingHeatDemand == 0)
                  {
                      gasMotor.usingHeatDemand = GasMotorUsingHeatDemand;
                  }
                 else if (gasMotor.operationCost == 0)
                  {
                    gasMotor.usingHeatDemand = 0;
                  }
                 
                newState["Gas motor"] = gasMotor;
                    SettingsForDynamicDataManualMode();
               }

     
            else if(name == "Electric boiler" && newState.TryGetValue("Electric boiler", out OPTLiveProp electricBoiler))
            {
                electricBoiler.stateOfUnit = ElectricBoilerState;
                electricBoiler.operationOfUnit = OilBoilerOperation;
                electricBoiler.UsageInPercentPerHour = ElectricBoilerOperationPercent;
                electricBoiler.operationCost = ElectricBoilerOperationCost;
                electricBoiler.usingCO2Emission = ElectricBoilerOperationCO2;

                if(electricBoiler.usingHeatDemand == 0)
                {
                    electricBoiler.usingHeatDemand = ElectricBoilerUsingHeatDemand;
                }
                else if (electricBoiler.operationCost == 0)
                {
                    electricBoiler.usingHeatDemand = 0;
                }

                newState["Electric boiler"] = electricBoiler;
                SettingsForDynamicDataManualMode();
            }
        }
          
        [RelayCommand]
        private void GasBoilerBehaviour()
            {
            GasBoilerTurnOn = !GasBoilerTurnOn;
            if(GasBoilerTurnOn)
            {
                GasBoilerState = "Green";
                GasBoilerOperation = "ON";
                GasBoilerOperationPercent = 100;
                GasBoilerOperationCost = 214;
                GasBoilerOperationCO2 = 1075;
                GasBoilerUsingHeatDemand = 5;

            }
            else
            {
                GasBoilerState = "Red";
                GasBoilerOperation = "OFF";
                GasBoilerOperationPercent = 0;
                GasBoilerOperationCO2 = 0;
                GasBoilerOperationCost = 0;
                GasBoilerUsingHeatDemand = 0;
            }
            UpdateListFromState("Gas boiler");
        }

        [RelayCommand]
        private void OilBoilerBehaviour()
        {
            OilBoilerTurnOn = !OilBoilerTurnOn;

            if (OilBoilerTurnOn)
            {
                OilBoilerState = "Green";
                OilBoilerOperation = "ON";
                OilBoilerOperationPercent = 100;
                OilBoilerOperationCost = 800;
                OilBoilerOperationCO2 = 1060;
                OilBoilerUsingHeatDemand = 4;
            }
            else
            {
                OilBoilerState = "Red";
                OilBoilerOperation = "OFF";
                OilBoilerOperationCO2 = 0;
                OilBoilerOperationCost = 0;
                OilBoilerOperationPercent = 0;
                OilBoilerUsingHeatDemand = 0;
            }

            UpdateListFromState("Oil boiler");
        }

        [RelayCommand]
        private void GasMotorBehaviour()
        {
            GasMotorTurnOn = !GasMotorTurnOn;

            if (GasMotorTurnOn)
            {
                GasMotorState = "Green";
                GasMotorOperation = "ON";
                GasMotorOperationPercent = 100;
                GasMotorOperationCost = 500;
                GasMotorOperationCO2 = 2304;
                GasMotorUsingHeatDemand = 3.6;
            }
            else
            {
                GasMotorState = "Red";
                GasMotorOperation = "OFF";
                GasMotorOperationPercent = 0;
                GasMotorOperationCO2 = 0;
                GasMotorOperationCost = 0;
                GasMotorUsingHeatDemand = 0;
            }

            UpdateListFromState("Gas motor");
        }

        [RelayCommand]
        private void ElectricBoilerBehaviour()
        {
            ElectricBoilerTurnOn = !ElectricBoilerTurnOn;

            if (ElectricBoilerTurnOn)
            {
                ElectricBoilerState = "Green";
                ElectricBoilerOperation = "ON";
                ElectricBoilerOperationPercent = 100;
                ElectricBoilerOperationCost = 300;
                ElectricBoilerOperationCO2 = 0;
                ElectricBoilerUsingHeatDemand = 8;
             
             }
            else
            {
                ElectricBoilerState = "Red";
                ElectricBoilerOperation = "OFF";
                ElectricBoilerOperationPercent = 0;
                ElectricBoilerOperationCO2 = 0;
                ElectricBoilerOperationCost = 0;
                ElectricBoilerUsingHeatDemand = 0;
            }
            UpdateListFromState("Electric boiler");
        }

        [RelayCommand]
        private void MinusHeatDemand(string name)
        {
            if(name == "Gas boiler" && newState.TryGetValue("Gas boiler", out OPTLiveProp gasboiler))
            {
                double maxHeat = gasboiler.data.MaxHeat;

                double percent5 = Math.Round(gasboiler.data.MaxHeat / 20, 3);

              
                if(GasBoilerUsingHeatDemand - percent5 >= 0)
                {
                    GasBoilerUsingHeatDemand = Math.Round(GasBoilerUsingHeatDemand - percent5, 3);
                    gasboiler.usingHeatDemand = Math.Round(gasboiler.usingHeatDemand - percent5, 3);
                }
          
                SettingsForDynamicDataManualMode();
            UpdateListFromState("Gas boiler");
            }
            else if(name == "Oil boiler" && newState.TryGetValue("Oil boiler", out OPTLiveProp oilboiler))
            {
                double maxHeat = oilboiler.data.MaxHeat;

                double percent5 = Math.Round(oilboiler.data.MaxHeat / 20, 3);

                if(OilBoilerUsingHeatDemand - percent5 >= 0)
                {
                    OilBoilerUsingHeatDemand = Math.Round(OilBoilerUsingHeatDemand - percent5, 3);
                    oilboiler.usingHeatDemand = Math.Round(oilboiler.usingHeatDemand - percent5, 3);
                }
                else
                {
                    OilBoilerUsingHeatDemand = 0;
                    oilboiler.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Oil boiler");
            }
            else if (name == "Gas motor" && newState.TryGetValue("Gas motor", out OPTLiveProp gasmotor))
            {
                double maxHeat = gasmotor.data.MaxHeat;

                double percent5 = Math.Round(gasmotor.data.MaxHeat / 20, 3);

                if (GasMotorUsingHeatDemand - percent5 >= 0)
                {
                    GasMotorUsingHeatDemand = Math.Round(GasMotorUsingHeatDemand - percent5, 3);
                    gasmotor.usingHeatDemand = Math.Round(gasmotor.usingHeatDemand - percent5, 3);
                }
                else
                {
                    GasMotorUsingHeatDemand = 0;
                    gasmotor.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Gas motor");
            }
            else if (name == "Electric boiler" && newState.TryGetValue("Electric boiler", out OPTLiveProp electricboiler))
            {
                double maxHeat = electricboiler.data.MaxHeat;

                double percent5 = Math.Round(electricboiler.data.MaxHeat / 20, 3);

                if (ElectricBoilerUsingHeatDemand - percent5 >= 0)
                {
                    ElectricBoilerUsingHeatDemand = Math.Round(ElectricBoilerUsingHeatDemand - percent5, 3);
                    electricboiler.usingHeatDemand = Math.Round(electricboiler.usingHeatDemand - percent5, 3);
                }
                else
                {
                    ElectricBoilerUsingHeatDemand = 0;
                    electricboiler.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Electric boiler");
            }
        }

        [RelayCommand]
        private void PlusHeatDemand(string name)
        {
            if (name == "Gas boiler" && newState.TryGetValue("Gas boiler", out OPTLiveProp gasboiler))
            {
                double maxHeat = gasboiler.data.MaxHeat;

                double percent5 = Math.Round(gasboiler.data.MaxHeat / 20, 3);


                if (GasBoilerUsingHeatDemand >= 0 && GasBoilerUsingHeatDemand < maxHeat)
                {
                    GasBoilerUsingHeatDemand = Math.Round(GasBoilerUsingHeatDemand + percent5, 3);
                    gasboiler.usingHeatDemand = Math.Round(gasboiler.usingHeatDemand + percent5, 3);
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Gas boiler");
            }
            else if (name == "Oil boiler" && newState.TryGetValue("Oil boiler", out OPTLiveProp oilboiler))
            {
                double maxHeat = oilboiler.data.MaxHeat;

                double percent5 = Math.Round(oilboiler.data.MaxHeat / 20, 3);

                if (OilBoilerUsingHeatDemand >= 0 && OilBoilerUsingHeatDemand < maxHeat)
                {
                    OilBoilerUsingHeatDemand = Math.Round(OilBoilerUsingHeatDemand + percent5, 3);
                    oilboiler.usingHeatDemand = Math.Round(oilboiler.usingHeatDemand + percent5, 3);
                }
                else
                {
                    OilBoilerUsingHeatDemand = 0;
                    oilboiler.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Oil boiler");
            }
            else if (name == "Gas motor" && newState.TryGetValue("Gas motor", out OPTLiveProp gasmotor))
            {
                double maxHeat = gasmotor.data.MaxHeat;

                double percent5 = Math.Round(gasmotor.data.MaxHeat / 20, 3);

                if (GasMotorUsingHeatDemand >= 0 && GasMotorUsingHeatDemand < maxHeat)
                {
                    GasMotorUsingHeatDemand = Math.Round(GasMotorUsingHeatDemand + percent5, 3);
                    gasmotor.usingHeatDemand = Math.Round(gasmotor.usingHeatDemand + percent5, 3);
                }
                else
                {
                    GasMotorUsingHeatDemand = 0;
                    gasmotor.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Gas motor");
            }
            else if (name == "Electric boiler" && newState.TryGetValue("Electric boiler", out OPTLiveProp electricboiler))
            {
                double maxHeat = electricboiler.data.MaxHeat;

                double percent5 = Math.Round(electricboiler.data.MaxHeat / 20, 3);

                if (ElectricBoilerUsingHeatDemand >= 0 && ElectricBoilerUsingHeatDemand < maxHeat)
                {
                    ElectricBoilerUsingHeatDemand = Math.Round(ElectricBoilerUsingHeatDemand + percent5, 3);
                    electricboiler.usingHeatDemand = Math.Round(electricboiler.usingHeatDemand + percent5, 3);
                }
                else
                {
                    ElectricBoilerUsingHeatDemand = 0;
                    electricboiler.usingHeatDemand = 0;
                }
                SettingsForDynamicDataManualMode();
                UpdateListFromState("Electric boiler");
            }
        }



        // relay commands for the low cost and eco friendly buttons
        [RelayCommand]
        private void LowCostTrue()
        {
            LowCost = !LowCost;

            if (LowCost)
            {
                LowCostWeight = "Bold";
                LowCostBackground = "Green";
                return;
            }

            if (LowCost != true && ECOFriendly == true)
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
        private void AutoMode(string? answer)
        {
            AutoModeSwitch = !AutoModeSwitch;

            if (answer == "yes")
            {
                viewChange.CurrentContent = new LiveOptimiser() { DataContext = new LiveOptimiserViewModel(viewChange) };
                return;
            }

        }

        private OPTLive oPTLive;

        public ManualModeLiveOptimiserViewModel(MainWindowViewModel viewChange)
        {
            this.viewChange = viewChange;
            optLive = new OPTLive("/Assets/data.xlsx");
            optLive.MachinesManualMode(160, true);
            HoldChangesNewList();
            SettingsForDynamicDataManualMode();
            }

        public void SettingsForDynamicDataManualMode()
        {
            ProductionCost = (int)optLive.ProductionCostPerHourManualMode(newState);
            CO2Emotions = (int)optLive.CO2EmmitionsPerHourManualMode(newState);

            var usagePerHour = optLive.UnitUsagePerHourManualMode(newState);
            GasBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
            OilBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
            GasMotorOperationPercent = usagePerHour.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
            ElectricBoilerOperationPercent = usagePerHour.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();

            HeatDemandCurrent = optLive.TotalHeatDemandManualMode(newState);
            HeatDemandPrediction = optLive.predictHeatDemandCalculate(160, true, "Assets/data.xlsx");
            PreviousHour = optLive.GetHourFromCellIndex(159, true, "Assets/data.xlsx");
            ActualHour = optLive.GetHourFromCellIndex(160, true, "Assets/data.xlsx");
            NextHour = optLive.GetHourFromCellIndex(161, true, "Assets/data.xlsx");

            var operationcost = optLive.UnitsOperationCostsManualMode(newState);
            GasBoilerOperationCost = operationcost.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
            OilBoilerOperationCost = operationcost.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
            GasMotorOperationCost = operationcost.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
            ElectricBoilerOperationCost = operationcost.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();


            var co2emissions = optLive.UnitsCO2EmissionManualMode(newState);
            GasBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Gas boiler").Select(unit => unit.Value).FirstOrDefault();
            OilBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Oil boiler").Select(unit => unit.Value).FirstOrDefault();
            GasMotorOperationCO2 = co2emissions.Where(unit => unit.Key == "Gas motor").Select(unit => unit.Value).FirstOrDefault();
            ElectricBoilerOperationCO2 = co2emissions.Where(unit => unit.Key == "Electric boiler").Select(unit => unit.Value).FirstOrDefault();
        }
    }
}
