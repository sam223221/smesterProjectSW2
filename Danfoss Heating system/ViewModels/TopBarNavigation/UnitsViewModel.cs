using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace Danfoss_Heating_system.ViewModels.TopBarNavigation
{
    public partial class UnitsViewModel : ViewModelBase
    {
        public class ProductionUnit
        {
            public string MaxHeat { get; set; }
            public string ProductionCost { get; set; }
            public string CO2Emission { get; set; }
            public string GasConsumption { get; set; }
            public string MaxElectricity { get; set; }
        }

        [ObservableProperty]
        private ProductionUnit currentUnit;

        public UnitsViewModel()
        {
            Units = new Dictionary<string, ProductionUnit>
            {
                ["GasBoiler"] = new ProductionUnit
                {
                    MaxHeat = "5,0 MW",
                    ProductionCost = "500 DKK/MWh",
                    CO2Emission = "215 kg/MWh",
                    GasConsumption = "1,1 MWh(gas)",
                    MaxElectricity = "none",
                },

                ["OilBoiler"] = new ProductionUnit
                {
                    MaxHeat = "4,0 MW",
                    ProductionCost = "700 DKK/MWh",
                    CO2Emission = "265 kg/MWh",
                    GasConsumption = "1,2 MWh(oil)",
                    MaxElectricity = "none",
                },

                ["GasMotor"] = new ProductionUnit
                {
                    MaxHeat = "3,6 MW",
                    ProductionCost = "1100 DKK/MWh",
                    CO2Emission = "640 kg/MWh",
                    GasConsumption = "1,9 MWh(gas)",
                    MaxElectricity = "2,7 MW",
                },

                ["ElectricBoiler"] = new ProductionUnit
                {
                    MaxHeat = "8,0 MW",
                    ProductionCost = "50 DKK/MWh",
                    CO2Emission = "none",
                    GasConsumption = "none",
                    MaxElectricity = "2,7 MW",
                },

                ["Empty"] = new ProductionUnit
                {
                    MaxHeat = "",
                    ProductionCost = "",
                    CO2Emission = "",
                    GasConsumption = "",
                    MaxElectricity = "",
                }
            };


            CurrentUnit = Units["Empty"];
        }

        public Dictionary<string, ProductionUnit> Units { get; }

        [ObservableProperty]
        private bool gasBoilerState = false;
        [ObservableProperty]
        private bool oilBoilerState = false;
        [ObservableProperty]
        private bool gasMotorState = false;
        [ObservableProperty]
        private bool electricBoilerState = false;

        [RelayCommand]
        private void SelectUnit(string unitKey)
        {
            if (Units.ContainsKey(unitKey))
            {
                CurrentUnit = Units[unitKey];
                OnPropertyChanged(nameof(CurrentUnit));

                switch (unitKey)
                {
                    case "GasBoiler":
                        GasBoilerState = true;
                        OilBoilerState = false;
                        GasMotorState = false;
                        ElectricBoilerState = false;
                        break;

                    case "OilBoiler":
                        GasBoilerState = false;
                        OilBoilerState = true;
                        GasMotorState = false;
                        ElectricBoilerState = false;
                        break;

                    case "GasMotor":
                        GasBoilerState = false;
                        OilBoilerState = false;
                        GasMotorState = true;
                        ElectricBoilerState = false;
                        break;

                    case "ElectricBoiler":
                        GasBoilerState = false;
                        OilBoilerState = false;
                        GasMotorState = false;
                        ElectricBoilerState = true;
                        break;

                }
            }
        }
    }
}
