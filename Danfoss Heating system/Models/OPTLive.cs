using Avalonia.Controls.Primitives;
using ClosedXML.Excel;
using Danfoss_Heating_system.ViewModels.OPT;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models;


/* 
input: 
1.newst date: datetime format  


outputs: list:      
1. which motors need to be trued on     
2. timer for each hour so 10 min before the whole hour it needs to trun on a motor     
3. current production     
4. lowest cost solution     
5. most eco friendly solution     
6. total cost     
7. current emmitions  manuale
*/




internal class OPTLive
{
    ExcelDataParser ExcelDataParser;
    public string filePath;
    
    Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> optimalizationDataStatus = new Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)>();
    
    Dictionary<string, OPTLiveProp> machinesForManualMode = new Dictionary<string, OPTLiveProp>();
    public OPTLive(string filepath)
    {
        string relativePath = @"..\..\..\Assets\data.xlsx";
        this.filePath = Path.GetFullPath(relativePath);
        this.ExcelDataParser = new ExcelDataParser(this.filePath);
    }

    // Optimizer (which units must be turn on)
    public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> UsingMachines(int hourDemandCell, bool winter)
    {
        var productionUnits = ExcelDataParser.ParserProductionUnits();
        var firstFourUnits = productionUnits.Take(4).ToList();
        double predictedHeatDemand = predictHeatDemandCalculate(hourDemandCell, winter, "/Assets/data.xlsx");

        foreach (var unit in firstFourUnits)
        {
            var localOPTProp = new OPTLiveProp(unit)
            {
                PredictedHeatDemand = predictedHeatDemand
            };


            if (unit.MaxHeat <= predictedHeatDemand)
            {
                localOPTProp.isUnitEnabled = true;
                predictedHeatDemand -= unit.MaxHeat;
                localOPTProp.usingHeatDemand = unit.MaxHeat;
                localOPTProp.usingCO2Emission = unit.CO2Emission * localOPTProp.usingHeatDemand;
                localOPTProp.stateOfUnit = "Green";
            }

            else if (predictedHeatDemand > 0 && predictedHeatDemand < unit.MaxHeat)
            {
                localOPTProp.isUnitEnabled = true;
                localOPTProp.usingHeatDemand = predictedHeatDemand;
                predictedHeatDemand = 0;
                localOPTProp.usingCO2Emission = localOPTProp.usingHeatDemand * unit.CO2Emission;
                localOPTProp.stateOfUnit = "Green";
            }
            else
            {
                localOPTProp.isUnitEnabled = false;
                localOPTProp.stateOfUnit = "Green";
            }

            if (optimalizationDataStatus.ContainsKey(unit.Name))
            {
                optimalizationDataStatus[unit.Name] = (localOPTProp.isUnitEnabled, localOPTProp);
            }
            else
            {
                optimalizationDataStatus.Add(unit.Name, (localOPTProp.isUnitEnabled, localOPTProp));

            }

        }
        return optimalizationDataStatus;
    }


    public Dictionary<string, OPTLiveProp> MachinesManualMode(int hourDemandCell, bool winter)
    {

        double predictedHeatDemand = predictHeatDemandCalculate(hourDemandCell, winter, "/Assets/data.xlsx");

        var productionUnits = ExcelDataParser.ParserProductionUnits();
        var firstFour = productionUnits.Take(4).ToList();

        foreach (var unit in firstFour)
        {
            var optLivePropForManual = new OPTLiveProp(unit)
            {
                PredictedHeatDemand = predictedHeatDemand
            };

            if (!machinesForManualMode.ContainsKey(unit.Name))
            {
                optLivePropForManual.isUnitEnabled = true;
                optLivePropForManual.usingHeatDemand = optLivePropForManual.usingHeatDemand;
                optLivePropForManual.usingCO2Emission = optLivePropForManual.usingHeatDemand * unit.CO2Emission;
                optLivePropForManual.stateOfUnit = "Green";
                optLivePropForManual.operationOfUnit = "ON";
                optLivePropForManual.UsageInPercentPerHour = 100;
                optLivePropForManual.operationCost = optLivePropForManual.operationCost;
                machinesForManualMode.Add(unit.Name, optLivePropForManual);
            }

        }
        return machinesForManualMode;
    }

    public double predictHeatDemandCalculate(int hourDemandCell, bool winter, string excelFilePath)
    {
        //Predicted heat demand
        double predHeatDemand = 0;
        //Offset of cells in the excel sheet
        int offset = 4;

        string cellOne = "D4";
        string cellTwo = "D4";
        string cellThr = "D4";

        double demand24;
        double demand48;
        double demand72;

        //Reference to the excel sheet
        using (var workbook = new XLWorkbook(this.filePath))
        {
            var worksheet = workbook.Worksheet("SDM");
            CultureInfo DanishInfo = new CultureInfo("da-DK");
             if (winter)
            {
                cellOne = "D" + (hourDemandCell - 24).ToString();
                cellTwo = "D" + (hourDemandCell - 48).ToString();
                cellThr = "D" + (hourDemandCell - 72).ToString();
            }
            else
            {
                cellOne = "I" + (hourDemandCell - 24).ToString();
                cellTwo = "I" + (hourDemandCell - 48).ToString();
                cellThr = "I" + (hourDemandCell - 72).ToString();
            }


        //This if statement makes the method return 0 as a heat demand if the data is insufficient
             if (hourDemandCell >= 24 + offset)
             {
                  //Getting the data from the excel cells
                  double.TryParse(worksheet.Cell(cellOne).GetValue<string>(),NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValueDemand24);
                demand24 = Math.Round(parsedValueDemand24, 2, MidpointRounding.AwayFromZero);
                double.TryParse(worksheet.Cell(cellTwo).GetValue<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValueDemand48);
                demand48 = Math.Round(parsedValueDemand48, 2, MidpointRounding.AwayFromZero);
                double.TryParse(worksheet.Cell(cellThr).GetValue<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValueDemand72);
                demand72 = Math.Round(parsedValueDemand72, 2, MidpointRounding.AwayFromZero);

                // This is here so that if we don't have enough heat demand data for past days, the program won't die
                if (hourDemandCell >= 72 + offset)
                    {
                      predHeatDemand = (demand24 + demand48 + demand72) / 3;
                    }
                  else if (hourDemandCell >= 48 + offset)
                    {
                      predHeatDemand = (demand24 + demand48) / 2;
                    }
                  else
                    {
                      predHeatDemand = demand24;
                    }
             }
        }
        double predictedHeatDemand = Math.Round(predHeatDemand, 2, MidpointRounding.AwayFromZero);
        return predictedHeatDemand;
    }


    // Lowest cost optimizer
    public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> LowestCost()
    {
        return optimalizationDataStatus.OrderBy(unit => unit.Value.OPTProp.data.ProductionCost)
                 .ToDictionary(unit => unit.Key, unit => unit.Value);
    }


    // Lowest cost for Manual Mode

    public Dictionary<string, OPTLiveProp> LowestCostManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        return newState.OrderBy(unit => unit.Value.data.ProductionCost)
                 .ToDictionary(unit => unit.Key, unit => unit.Value);
    }


    // Most eco friendly

    public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> EcoFriendly()
    {
        return optimalizationDataStatus.OrderBy(unit => unit.Value.OPTProp.data.CO2Emission)
            .ToDictionary(unit => unit.Key, unit => unit.Value);
    }



    // Total Production Cost

    public double ProductionCostPerHour()
    {

        double TotalOneHourCost = 0;
        foreach (var unit in optimalizationDataStatus)
        {

            TotalOneHourCost += unit.Value.OPTProp.usingHeatDemand * unit.Value.OPTProp.data.ProductionCost;
        }
        return TotalOneHourCost;
    }


    // Total production cost for manual mode
    public double ProductionCostPerHourManualMode(Dictionary<string, OPTLiveProp> newState)
    {

        double TotalOneHourCost = 0;
        foreach (var unit in newState)
        {

            TotalOneHourCost += unit.Value.usingHeatDemand * unit.Value.data.ProductionCost;
        }
        return TotalOneHourCost;
    }


    // Total CO2 Emmition 
    public double CO2EmmitionsPerHour()
    {
        double TotalCO2Emmition = 0;
        foreach (var unit in optimalizationDataStatus)
        {
            TotalCO2Emmition += unit.Value.OPTProp.usingCO2Emission;
        }
        return TotalCO2Emmition;
    }


    //Total CO2 Emmition for manual mode

    public double CO2EmmitionsPerHourManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        double TotalCO2Emmition = 0;
        foreach (var unit in newState)
        {
            TotalCO2Emmition += unit.Value.usingCO2Emission;
        }
        return TotalCO2Emmition;
    }



    // Percent of using each unit
    public Dictionary<string, int> UnitUsagePerHour()
    {
        Dictionary<string, int> PercentUsage = new();
        foreach (var unit in optimalizationDataStatus)
        {
            if (unit.Value.OPTProp.usingHeatDemand != 0)
            {
                unit.Value.OPTProp.UsageInPercentPerHour = (int)(((double)unit.Value.OPTProp.usingHeatDemand / (double)unit.Value.OPTProp.data.MaxHeat) * 100);
                PercentUsage.Add(unit.Value.OPTProp.data.Name, unit.Value.OPTProp.UsageInPercentPerHour);
            }
            else
            {
                PercentUsage.Add(unit.Value.OPTProp.data.Name, 0);
            }
        }

        return PercentUsage;
    }

    // Percent of using each unit Manual Mode
    public Dictionary<string, int> UnitUsagePerHourManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        Dictionary<string, int> PercentUsage = new();
        foreach (var unit in newState)
        {
            if (unit.Value.usingHeatDemand != 0)
            {
                unit.Value.UsageInPercentPerHour = (int)(((double)unit.Value.usingHeatDemand / (double)unit.Value.data.MaxHeat) * 100);
                PercentUsage.Add(unit.Value.data.Name, unit.Value.UsageInPercentPerHour);
            }
            else
            {
                PercentUsage.Add(unit.Value.data.Name, 0);
            }
        }

        return PercentUsage;
    }


   // Total Heat Demand
    public double TotalHeatDemand()
    {
        double HeatDemand = 0;

        foreach (var unit in optimalizationDataStatus)
        {
            HeatDemand += unit.Value.OPTProp.usingHeatDemand;
        }
        double roundedHeatDemand = Math.Round(HeatDemand, 2);
        return roundedHeatDemand;
    }


    //Total Heat Demand Manual Mode
    public double TotalHeatDemandManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        double HeatDemand = 0;

        foreach (var unit in newState)
        {
            HeatDemand += unit.Value.usingHeatDemand;
        }
        double roundedHeatDemand = Math.Round(HeatDemand, 3);
        return roundedHeatDemand;
    }

    // Units Operation Costs
    public Dictionary<string, int> UnitsOperationCosts()
    {
        Dictionary<string, int> OperationUnitsCosts = new();

        foreach (var unit in optimalizationDataStatus)
        {
            int price = Convert.ToInt32(unit.Value.OPTProp.usingHeatDemand * unit.Value.OPTProp.data.ProductionCost);
            OperationUnitsCosts.Add(unit.Value.OPTProp.data.Name, price);
        }
        return OperationUnitsCosts;
    }


    //Units Operation Costs Manual Mode
    public Dictionary<string, int> UnitsOperationCostsManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        Dictionary<string, int> OperationUnitsCosts = new();

        foreach (var unit in newState)
        {
            int price = Convert.ToInt32(unit.Value.usingHeatDemand * unit.Value.data.ProductionCost);
            OperationUnitsCosts.Add(unit.Value.data.Name, price);
        }
        return OperationUnitsCosts;
    }


    // Units CO2 Emmision
    public Dictionary<string, int> UnitsCO2Emission()
    {
        Dictionary<string, int> unitsCO2Emissions = new();

        foreach (var unit in optimalizationDataStatus)
        {
            int co2 = Convert.ToInt32(unit.Value.OPTProp.usingCO2Emission);
            unitsCO2Emissions.Add(unit.Value.OPTProp.data.Name, co2);
        }
        return unitsCO2Emissions;
    }


    // Units CO2 Emmision Manual Mode
    public Dictionary<string, int> UnitsCO2EmissionManualMode(Dictionary<string, OPTLiveProp> newState)
    {
        Dictionary<string, int> unitsCO2Emissions = new();

        foreach (var unit in newState)
        {
            int co2 = Convert.ToInt32(unit.Value.usingHeatDemand * unit.Value.data.CO2Emission);
            unitsCO2Emissions.Add(unit.Value.data.Name, co2);
        }
        return unitsCO2Emissions;
    }



    public Dictionary<string, (string, string)> StateOfUnits(int currentHour, bool isWinter, string FilePath)
    {
        Dictionary<string, (string, string)> StateofUnits = new();

        foreach(var unit in optimalizationDataStatus)
        {
            if(unit.Value.isEnabled == true)
            {
                unit.Value.OPTProp.stateOfUnit = "Green";
                unit.Value.OPTProp.operationOfUnit = "ON";
            }
           
            else 
            {
            // finish this
                double currentDemand = predictHeatDemandCalculate(currentHour, isWinter, FilePath);
                double nextHourPredictedDemand = NextHourPredictedDemand(currentHour, isWinter, FilePath);

                if(nextHourPredictedDemand - currentDemand > 2)
                {
                    unit.Value.OPTProp.stateOfUnit = "Orange";
                    unit.Value.OPTProp.operationOfUnit = "Heating up";
                }
                else
                {
                unit.Value.OPTProp.stateOfUnit = "Gray";
                unit.Value.OPTProp.operationOfUnit = "OFF";
                }
            }
            StateofUnits.Add(unit.Value.OPTProp.data.Name, (unit.Value.OPTProp.stateOfUnit, unit.Value.OPTProp.operationOfUnit));
     
        }
        return StateofUnits;
    }



    public double NextHourPredictedDemand(int hourDemandCell, bool isWinter, string filepath)
    {
        double currentDemand = predictHeatDemandCalculate(hourDemandCell, isWinter, filepath);

        // change the numbers
        double changeFactor = isWinter ? 1.05 : 1.05;

        double nextHourDemand = currentDemand * changeFactor;
        // 5 * 1.5 8
        return nextHourDemand;
    }

    public string GetHourFromCellIndex(int cellIndex, bool winter, string filepath)
    {
        int adjustedIndex = cellIndex + 3;

        using (var workbook = new XLWorkbook(filepath))
        {
            var worksheet = workbook.Worksheet("SDM");
            string column = winter ? "B" : "G";

            var cellValue = worksheet.Cell(adjustedIndex, column).GetValue<string>().Trim();

            if (DateTime.TryParse(cellValue, out DateTime parsedDate)) 
            {
                return parsedDate.ToString("HH:mm");
            }
            return parsedDate.ToString("HH:mm");
        }
    }
}
