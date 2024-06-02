using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Danfoss_Heating_system.Models;



internal class OPTLive
{
    ExcelDataParser excelDataParser;

    public string filePath;
    public double currentHeatDemand;
    public double productionCost;
    public double CO2Emission;

    List<OPTLiveProp> machinesForManualMode = new();

    public OPTLive(string filepath)
    {
        string relativePath = @"..\..\..\Assets\data.xlsx";
        this.filePath = Path.GetFullPath(relativePath);
        this.excelDataParser = new ExcelDataParser(this.filePath);
    }



    public List<OPTLiveProp> UsingMachines(bool winter, string scenario)
    {
        var productionUnits = excelDataParser.ParserProductionUnits();
        double predictedHeatDemand = predictHeatDemandCalculate(winter);

        string season = winter ? "Winter" : "Summer";
        var seasonList = excelDataParser.ParserEnergyData().Where(p => p.Season == season).ToList();
        var energyData = seasonList.Last();

        // Pre-calculate the production cost for the electric boiler and Gas Motor
        foreach (var unit in productionUnits)
        {
            if (unit.Name == "Electric boiler")
            {
                unit.ProductionCost += energyData.ElectricityPrice;
            }
            if (unit.Name == "Gas motor")
            {
                double heatToProduce = Math.Min(energyData.HeatDemand, 3.6);
                double totalHeatCost = heatToProduce * unit.ProductionCost;
                double electricityProduced = (heatToProduce / 3.6) * unit.MaxElectricity;
                double totalElectricityRevenue = electricityProduced * energyData.ElectricityPrice;
                unit.ProductionCost = totalHeatCost - totalElectricityRevenue;
            }
        }

        // Sort production units based on the scenario
        switch (scenario)
        {
            case "BestCost":
                productionUnits = productionUnits.OrderBy(p => p.ProductionCost).ToList();
                break;

            case "LowestCO2":
                productionUnits = productionUnits.OrderBy(p => p.CO2Emission / p.MaxHeat).ToList();
                break;
        }

        double remainingHeat = energyData.HeatDemand;
        var unitsUsed = new List<string>();
        var usedUnits = new HashSet<string>();
        var units = new List<OPTLiveProp>();
        double unitProductionCost = 0;
        double unitCO2Emission = 0;

        foreach (var unit in productionUnits)
        {
            var Unit = new OPTLiveProp()
            {
                NameOfUnit = unit.Name,
                isUnitEnabled = false,
                operationCost = 0,
                usingCO2Emission = 0,
                stateOfUnit = "Red",
                operationOfUnit = "OFF",
                UsageInPercentPerHour = 0
            };

            if (remainingHeat <= 0) { units.Add(Unit); continue; }
            if (usedUnits.Contains(unit.Name)) continue;

            double heatToProduce = Math.Min(remainingHeat, unit.MaxHeat);
            remainingHeat -= heatToProduce;
            unitProductionCost += heatToProduce * unit.ProductionCost;
            unitCO2Emission += heatToProduce * unit.CO2Emission;

            Unit.isUnitEnabled = true;
            Unit.usingHeatDemand = heatToProduce;
            Unit.usingCO2Emission = heatToProduce * unit.CO2Emission;
            Unit.operationCost = heatToProduce * unit.ProductionCost;
            Unit.stateOfUnit = "Green";
            Unit.operationOfUnit = "ON";
            Unit.UsageInPercentPerHour = (int)((heatToProduce / unit.MaxHeat) * 100);

            unitsUsed.Add($"{unit.Name} {heatToProduce:F2}MW");
            usedUnits.Add(unit.Name);
            units.Add(Unit);
        }

        currentHeatDemand = energyData.HeatDemand;
        CO2Emission = unitCO2Emission;
        productionCost = unitProductionCost;

        return units;
    }




    public List<OPTLiveProp> MachinesInitialize(EnergyData hourInfo, bool isWinter)
    {
        machinesForManualMode.Clear();
        
        var storedUnits = excelDataParser.ParserUnitsState();
        var productionUnits = excelDataParser.ParserProductionUnits();

        // Pre-calculate the production cost for the electric boiler and Gas Motor
        foreach (var unit in productionUnits)
        {
            if (unit.Name == "Electric boiler")
            {
                unit.ProductionCost += hourInfo.ElectricityPrice;
            }
            if (unit.Name == "Gas motor")
            {
                double heatToProduce = Math.Min(hourInfo.HeatDemand, 3.6);
                double totalHeatCost = heatToProduce * unit.ProductionCost;
                double electricityProduced = (heatToProduce / 3.6) * unit.MaxElectricity;
                double totalElectricityRevenue = electricityProduced * hourInfo.ElectricityPrice;
                unit.ProductionCost = totalHeatCost - totalElectricityRevenue;
            }
        }

        foreach (var storedUnit in storedUnits)
        {
            var UnitCreation = new OPTLiveProp();

            UnitCreation.NameOfUnit = storedUnit.NameOfUnit;
            UnitCreation.stateOfUnit = storedUnit.stateOfUnit;
            UnitCreation.operationOfUnit = storedUnit.operationOfUnit;
            UnitCreation.UsageInPercentPerHour = storedUnit.UsageInPercentPerHour;
            UnitCreation.operationCost = storedUnit.operationCost;
            UnitCreation.usingCO2Emission = storedUnit.usingCO2Emission;
            UnitCreation.usingHeatDemand = storedUnit.usingHeatDemand;
            UnitCreation.isUnitEnabled = storedUnit.isUnitEnabled;

            UnitCreation.data = productionUnits.Find(p => p.Name == storedUnit.NameOfUnit);

            machinesForManualMode.Add(UnitCreation);

        }
        return isWinter ? machinesForManualMode.Take(4).ToList() : machinesForManualMode.Skip(4).Take(4).ToList();
    }



    // Total production cost for manual mode
    public double ProductionCostPerHourManualMode(List<OPTLiveProp> newState)
    {

        double TotalOneHourCost = 0;
        foreach (var unit in newState)
        {
            TotalOneHourCost += unit.data.ProductionCost * unit.usingHeatDemand;
        }
        return TotalOneHourCost;
    }


    //Total CO2 Emmition for manual mode
    public double CO2EmmitionsPerHourManualMode(List<OPTLiveProp> newState)
    {
        double TotalCO2Emmition = 0;
        foreach (var unit in newState)
        {
            TotalCO2Emmition += unit.data.CO2Emission * unit.usingHeatDemand;
        }
        return TotalCO2Emmition;
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

    public void StoreUnitState(List<OPTLiveProp> store, bool isWinter)
    {

        excelDataParser.UpdateUnitsInWorksheet(store, isWinter);
    }


    //Total Heat Demand Manual Mode
    public double TotalHeatProductionManualMode(List<OPTLiveProp> newState)
    {
        double HeatDemand = 0;

        foreach (var unit in newState)
        {
            HeatDemand += unit.usingHeatDemand;
        }
        return HeatDemand;
    }
    public EnergyData HourInformation(bool iswinter)
    {
        var season = iswinter ? "Winter" : "Summer";

        var groupBySeason = excelDataParser.ParserEnergyData().Where(p => p.Season == season).ToList();

        int adjustedIndex = groupBySeason.Count();

        return groupBySeason[adjustedIndex - 1];
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




    public double NextHourPredictedDemand(bool isWinter)
    {
        double currentDemand = predictHeatDemandCalculate(isWinter);

        // change the numbers
        double changeFactor = isWinter ? 1.05 : 1.05;

        double nextHourDemand = currentDemand * changeFactor;
        // 5 * 1.5 8
        return nextHourDemand;
    }

    public string GetHourFromCellIndex(int whichClock, bool winter)
    {
        string season = winter ? "Winter" : "Summer";
        var groupBySeason = excelDataParser.ParserEnergyData().Where(p => p.Season == season).ToList();

        int adjustedIndex = groupBySeason.Count();

        if (whichClock == 0)
        {
            var preHour = groupBySeason[adjustedIndex - 1].TimeFrom.AddHours(-1);
            return preHour.ToString("HH:mm");
        }
        else if (whichClock == 1)
        {
            return groupBySeason[adjustedIndex - 1].TimeFrom.ToString("HH:mm");
        }
        else
        {
            var nextHour = groupBySeason[adjustedIndex - 1].TimeFrom.AddHours(1);
            return nextHour.ToString("HH:mm");
        }
    }








    public double predictHeatDemandCalculate(bool winter)
    {
        string season = winter ? "Winter" : "Summer";
        int index = excelDataParser.ParserEnergyData().Where(p => p.Season == season).ToList().Count() + 3;

        //Predicted heat demand
        double predHeatDemand = 0;

        string cellOne;
        string cellTwo;
        string cellThr;

        double demand24;
        double demand48;
        double demand72;

        //Reference to the excel sheet
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet("SDM");
            CultureInfo DanishInfo = new CultureInfo("da-DK");
            if (winter)
            {
                cellOne = "D" + (index - 24).ToString();
                cellTwo = "D" + (index - 48).ToString();
                cellThr = "D" + (index - 72).ToString();
            }
            else
            {
                cellOne = "I" + (index - 24).ToString();
                cellTwo = "I" + (index - 48).ToString();
                cellThr = "I" + (index - 72).ToString();
            }


            //This if statement makes the method return 0 as a heat demand if the data is insufficient
            if (index >= 24)
            {
                //Getting the data from the excel cells
                double.TryParse(worksheet.Cell(cellOne).GetValue<string>(), NumberStyles.Any, DanishInfo, out double parsedValueDemand24);
                demand24 = Math.Round(parsedValueDemand24, 2, MidpointRounding.AwayFromZero);
                double.TryParse(worksheet.Cell(cellTwo).GetValue<string>(), NumberStyles.Any, DanishInfo, out double parsedValueDemand48);
                demand48 = Math.Round(parsedValueDemand48, 2, MidpointRounding.AwayFromZero);
                double.TryParse(worksheet.Cell(cellThr).GetValue<string>(), NumberStyles.Any, DanishInfo, out double parsedValueDemand72);
                demand72 = Math.Round(parsedValueDemand72, 2, MidpointRounding.AwayFromZero);

                // This is here so that if we don't have enough heat demand data for past days, the program won't die
                if (index >= 72)
                {
                    predHeatDemand = (demand24 + demand48 + demand72) / 3;
                }
                else if (index >= 48)
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
}
