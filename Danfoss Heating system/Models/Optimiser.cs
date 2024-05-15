using System;
using System.Collections.Generic;
using System.Linq;

namespace Danfoss_Heating_system.Models
{
    public class Optimiser
    {
        ExcelDataParser excelDataParser;

        public Optimiser(string filePath)
        {
            excelDataParser = new ExcelDataParser(filePath);
        }

        public enum OptimizationType
        {
            BestCost,
            LowestCO2
        }

        public enum OptimizationScenario
        {
            Scenario1,
            Scenario2
        }

        public List<OperationResult> CalculateOptimalOperations(DateTime specificDate, OptimizationScenario scenario, OptimizationType optimizationType)
        {
            var results = new List<OperationResult>();
            var energyData = excelDataParser.ParserEnergyData()
                                .FirstOrDefault(data => data.TimeFrom == specificDate);

            if (energyData == null)
            {
                throw new ArgumentException("No energy data found for the specified date.");
            }

            var productionUnits = excelDataParser.ParserProductionUnits();
            var result = new OperationResult();

            // Define thresholds based on observed data
            double highPriceThreshold = 1200; // Example high price threshold
            double lowPriceThreshold = 700;   // Example low price threshold

            // Sort units based on the optimization type
            List<EnergyData> orderedUnits;
            if (optimizationType == OptimizationType.BestCost)
            {
                if (scenario == OptimizationScenario.Scenario1)
                {
                    // Scenario 1: Prioritize gas boiler and use oil boiler if needed
                    orderedUnits = productionUnits
                        .Where(p => p.Name == "Gas boiler" || p.Name == "Oil boiler")
                        .OrderBy(p => p.ProductionCost / p.MaxHeat)
                        .ToList();
                }
                else
                {
                    // Scenario 2: Use cost-effectiveness and incorporate dynamic electricity prices
                    orderedUnits = productionUnits.OrderBy(p => p.ProductionCost / p.MaxHeat).ToList();
                }
            }
            else // OptimizationType.LowestCO2
            {
                orderedUnits = productionUnits.OrderBy(p => p.CO2Emission / p.MaxHeat).ToList();
            }

            double requiredHeat = energyData.HeatDemand;
            var unitsUsed = new List<string>();

            foreach (var unit in orderedUnits)
            {
                if (requiredHeat <= 0) break;

                double usedHeat;
                double cost;
                double emissions;

                if (scenario == OptimizationScenario.Scenario2)
                {
                    // Special logic for gas motor and electric boiler in Scenario 2
                    if (unit.Name == "Gas motor" && energyData.ElectricityPrice > highPriceThreshold)
                    {
                        // Use gas motor if electricity prices are high
                        usedHeat = Math.Min(unit.MaxHeat, requiredHeat);
                        cost = usedHeat * unit.ProductionCost;
                        emissions = usedHeat * unit.CO2Emission;

                        unitsUsed.Add(unit.Name);
                        result.TotalCost += cost;
                        result.CO2Emissions += emissions;
                        requiredHeat -= usedHeat;
                        continue;
                    }
                    else if (unit.Name == "Electric boiler" && energyData.ElectricityPrice < lowPriceThreshold)
                    {
                        // Use electric boiler if electricity prices are low
                        usedHeat = Math.Min(unit.MaxHeat, requiredHeat);
                        // Calculate the cost considering electricity consumption
                        cost = usedHeat * unit.ProductionCost + usedHeat * Math.Abs(unit.MaxElectricity) * energyData.ElectricityPrice;
                        emissions = usedHeat * unit.CO2Emission;

                        unitsUsed.Add(unit.Name);
                        result.TotalCost += cost;
                        result.CO2Emissions += emissions;
                        requiredHeat -= usedHeat;
                        continue;
                    }
                }

                // General logic for other units
                usedHeat = Math.Min(unit.MaxHeat, requiredHeat);
                cost = usedHeat * unit.ProductionCost;
                emissions = usedHeat * unit.CO2Emission;

                unitsUsed.Add(unit.Name);
                result.TotalCost += cost;
                result.CO2Emissions += emissions;
                requiredHeat -= usedHeat;
            }

            result.UnitsUsed = string.Join(", ", unitsUsed);
            result.HeatDemand = energyData.HeatDemand;
            result.ElectricityPrice = energyData.ElectricityPrice;
            results.Add(result);

            return results;
        }
    }
}
