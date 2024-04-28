using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;

namespace Danfoss_Heating_system.ViewModels.OPT
{
    public partial class ManualModeLiveOptimiserViewModel : ViewModelBase
    {

        private MainWindowViewModel viewChange;
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
        private int gasBoilerOperationPercent = 100;
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
        private bool autoModeSwitch = false;



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
            oPTLive = new OPTLive("Assets/data.xlsx");
        }

       
    }
}
