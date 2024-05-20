using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static Danfoss_Heating_system.Models.Optimiser;

namespace Danfoss_Heating_system.ViewModels.OPT
{
    public partial class GraphOptimiserViewModel : ViewModelBase
    {
        // ----------- Properties for Bindings ----------- //

        [ObservableProperty]
        private double actualWidth;
        [ObservableProperty]
        private double actualHeight;
        [ObservableProperty]
        private string winterButtonForeground = "Green";
        [ObservableProperty]
        private string winterButtonBackground = "LightGreen";
        [ObservableProperty]
        private int selectedIndex = -1;
        [ObservableProperty]
        private int? name;
        [ObservableProperty]
        private string resultCurrentHeatDemand = "";
        [ObservableProperty]
        private string resultCurrentElectricalPrice = "";
        [ObservableProperty]
        private string resultCurrentProfit = "";
        [ObservableProperty]
        private string resultMotorsInUse = "";
        [ObservableProperty]
        private string summerButtonForeground = "Red";
        [ObservableProperty]
        private string summerButtonBackground = "LightCoral";
        [ObservableProperty]
        private string bestCostForeground= "Green";
        [ObservableProperty]
        private string bestCostBackground = "LightGreen";
        [ObservableProperty]
        private string lowestCo2Foreground = "Red";
        [ObservableProperty]
        private string lowestCo2Background = "LightCoral";
        [ObservableProperty]
        private string scenario1Foreground = "Red";
        [ObservableProperty]
        private string scenario1Background = "LightCoral";
        [ObservableProperty]
        private string scenario2Foreground = "Red";
        [ObservableProperty]
        private string scenario2Background = "LightCoral";


        // ----------- List for Variables ----------- //

        private List<EnergyData> allData;
        private MainWindowViewModel viewChange;
        private SortingTheDaysInExcelParser summerData = new SortingTheDaysInExcelParser();
        private SortingTheDaysInExcelParser winterData = new SortingTheDaysInExcelParser();
        private SortingTheDaysInExcelParserProps displayedData = new();
        private Dictionary<DateTime, List<EnergyData>> summerPeriodData;
        private Dictionary<DateTime, List<EnergyData>> winterPeriodData;
        private List<EnergyData> summerPeriod = new List<EnergyData>();
        private List<EnergyData> winterPeriod = new List<EnergyData>();
        private EnergyData? selectedDate;
        private bool isThereData = false;
        public ObservableCollection<SortingTheDaysInExcelParserProps> DataDisplayed { get; set; } = new();
        public ObservableCollection<GraphProps> DataGraphDisplayed { get; set; } = new();
        private Optimiser optimizer;
        private OptimizationType optimizationType = OptimizationType.BestCost;

        // Constructor
        public GraphOptimiserViewModel(MainWindowViewModel mv)
        {
            viewChange = mv;

            // Subscribe to window size changes to update actualWidth and actualHeight
            mv.window.GetObservable(Window.ClientSizeProperty).Subscribe(size =>
            {
                ActualWidth = (int)size.Width;
                ActualHeight = size.Height;
            });

            // Set window properties
            viewChange.window.CanResize = true;
            viewChange.window.Width = 1980;
            viewChange.window.Height = 1020;

            // Parse all data from the Excel file
            allData = new ExcelDataParser("Assets/data.xlsx").ParserEnergyData();

            // Initialize data and construct initial winter period data
            DataInitializer();
            WinterPeriodDataConstructor(null);

            // initilizing the Optimiser
            optimizer = new Optimiser("Assets/data.xlsx");

        }

        // ----------- RelayCommands for Buttons ----------- //

        [RelayCommand]
        private void WinterButton()
        {
            WinterButtonForeground = "Green";
            WinterButtonBackground = "LightGreen";
            SummerButtonForeground = "Red";
            SummerButtonBackground = "LightCoral";

            WinterPeriodDataConstructor(null);
        }

        [RelayCommand]
        private void SummerButton()
        {
            WinterButtonForeground = "Red";
            WinterButtonBackground = "LightCoral";
            SummerButtonForeground = "Green";
            SummerButtonBackground = "LightGreen";

            SummerPeriodDataConstructor(null);
        }

        [RelayCommand]
        private void BestCost()
        {
            if (!isThereData) return;

            BestCostForeground ="Green";
            BestCostBackground = "LightGreen";
            LowestCo2Foreground = "Red";
            LowestCo2Background = "LightCoral";
            Scenario2Foreground = "Red";
            Scenario2Background = "LightCoral";
            Scenario1Foreground = "Red";
            Scenario1Background = "LightCoral";

            optimizationType = OptimizationType.BestCost;
            ResultDataUpdate(selectedDate.TimeFrom);
        }

        [RelayCommand]
        private void LowestCo2()
        {
            if (!isThereData) return;

            BestCostForeground = "Red";
            BestCostBackground = "LightCoral";
            LowestCo2Foreground = "Green";
            LowestCo2Background = "LightGreen";
            Scenario2Foreground = "Red";
            Scenario2Background = "LightCoral";
            Scenario1Foreground = "Red";
            Scenario1Background = "LightCoral";

            optimizationType = OptimizationType.LowestCO2;
            ResultDataUpdate(selectedDate.TimeFrom);
        }

        [RelayCommand]
        private void Scenario1()
        {
            if (!isThereData) return;

            Scenario1Foreground = "Green";
            Scenario1Background = "LightGreen";
            Scenario2Foreground = "Red";
            Scenario2Background = "LightCoral";
            BestCostForeground = "Red";
            BestCostBackground = "LightCoral";
            LowestCo2Foreground = "Red";
            LowestCo2Background = "LightCoral";

            optimizationType = OptimizationType.Scenario1;
            ResultDataUpdate(selectedDate.TimeFrom);
        }

        [RelayCommand] 
        private void Scenario2() 
        {
            if (!isThereData) return;

            Scenario1Foreground = "Red";
            Scenario1Background = "LightCoral";
            Scenario2Foreground = "Green";
            Scenario2Background = "LightGreen";
            BestCostForeground = "Red";
            BestCostBackground = "LightCoral";
            LowestCo2Foreground = "Red";
            LowestCo2Background = "LightCoral";

            optimizationType = OptimizationType.Scenario2;
            ResultDataUpdate(selectedDate.TimeFrom);
        }


        [RelayCommand]
        private void PilerInfo(string? info)
        {
            isThereData = true;

            foreach (var item in displayedData.data)
            {
                if (item.TimeFrom.ToString() == info)
                {
                    selectedDate = item;

                    ResultDataUpdate(selectedDate.TimeFrom);

                    UpdateGraph(selectedDate.TimeFrom.ToString());

                }
            }
        }

        // ----------- Methods ----------- //

        // Initialize data by sorting into winter and summer periods
        private void DataInitializer()
        {
            foreach (var item in allData)
            {
                if (item.Season == "Winter")
                {
                    winterPeriod.Add(item);
                }
                if (item.Season == "Summer")
                {
                    summerPeriod.Add(item);
                }
            }
            summerPeriodData = summerData.SortDataByDate(summerPeriod);
        }

        private void ResultDataUpdate(DateTime timeFrom)
        {
            
            var result = optimizer.CalculateOptimalOperations(timeFrom, optimizationType);
            ResultCurrentHeatDemand = result[0].HeatDemand.ToString("F2");
            ResultCurrentElectricalPrice = result[0].ElectricityPrice.ToString("F2");
            ResultCurrentProfit = result[0].TotalCost.ToString("F2");
            ResultMotorsInUse = result[0].UnitsUsed;
        }

        // Construct data for winter period
        private void WinterPeriodDataConstructor(string? chosenIndex)
        {
            DataDisplayed.Clear();
            winterPeriodData = winterData.SortDataByDate(winterPeriod);

            foreach (var item in winterPeriodData)
            {
                DataDisplayed.Add(new SortingTheDaysInExcelParserProps { date = item.Key.Date.ToString("dd/MM/yyyy"), data = item.Value });

            }
        }

        // Construct data for summer period
        private void SummerPeriodDataConstructor(string? chosenIndex)
        {
            DataDisplayed.Clear();

            foreach (var item in summerPeriodData)
            {

               DataDisplayed.Add(new SortingTheDaysInExcelParserProps { date = item.Key.Date.ToString("dd/MM/yyyy"), data = item.Value });

            }
        }
        // Handle selection index change
        partial void OnSelectedIndexChanged(int value)
        {

            if (value == -1) return;
            
            displayedData = DataDisplayed[value];

            UpdateGraph(null);
        }

        private void UpdateGraph(string? selectedData)
        {
            DataGraphDisplayed.Clear();

            foreach (var item in displayedData.data)
            {
                if (item.TimeFrom.ToString() != selectedData)
                {
                    DataGraphDisplayed.Add(new GraphProps((item.HeatDemand / 10) * 800, item.TimeFrom.ToString(), ReactiveCommand.Create<String>(PilerInfo), "DarkGreen", item.HeatDemand.ToString("F1")));
                }
                else
                {
                    DataGraphDisplayed.Add(new GraphProps((item.HeatDemand / 10) * 800, item.TimeFrom.ToString(), ReactiveCommand.Create<String>(PilerInfo), "DarkSeaGreen", item.HeatDemand.ToString("F1")));
                }
            }
        }

    }
}
