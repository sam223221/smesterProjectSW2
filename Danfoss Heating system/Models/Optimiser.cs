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
            LowestCO2,
            Scenario1,
            Scenario2
        }

        public List<OperationResult> CalculateOptimalOperations(DateTime specificDate, OptimizationType optimizationType)
        {
            var results = new List<OperationResult>();
            var energyData = excelDataParser.ParserEnergyData()
                                .FirstOrDefault(data => data.TimeFrom == specificDate);

            if (energyData == null)
            {
                throw new ArgumentException("No energy data found for the specified date.");
            }

            var productionUnits = excelDataParser.ParserProductionUnits();
            var result = new OperationResult
            {
                Date = specificDate,
                HeatDemand = energyData.HeatDemand,
                ElectricityPrice = energyData.ElectricityPrice
            };

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

            var orderedUnits = SortProductionUnits(productionUnits, optimizationType, energyData);
            double remainingHeat = energyData.HeatDemand;
            var unitsUsed = new List<string>();
            var usedUnits = new HashSet<string>();

            foreach (var unit in orderedUnits)
            {
                if (remainingHeat <= 0) break;
                if (usedUnits.Contains(unit.Name)) continue;

                double heatToProduce = Math.Min(remainingHeat, unit.MaxHeat);
                remainingHeat -= heatToProduce;

                result.TotalCost += CalculateCost(unit, heatToProduce);
                result.CO2Emissions += CalculateEmissions(unit, heatToProduce);
                unitsUsed.Add(unit.Name + " " + heatToProduce.ToString("F2") + "MW");
                usedUnits.Add(unit.Name);
            }

            if (remainingHeat > 0)
            {
                throw new InvalidOperationException("Unable to meet the heat demand with the available units.");
            }

            result.UnitsUsed = string.Join(" : ", unitsUsed);
            results.Add(result);

            return results;
        }

        private List<EnergyData> SortProductionUnits(IEnumerable<EnergyData> productionUnits, OptimizationType optimizationType, EnergyData electricalPrice)
        {
            switch (optimizationType)
            {
                case OptimizationType.BestCost:
                    return productionUnits.OrderBy(p => p.ProductionCost).ToList();
                case OptimizationType.LowestCO2:
                    return productionUnits.OrderBy(p => p.CO2Emission / p.MaxHeat).ToList();
                case OptimizationType.Scenario1:
                    return productionUnits
                        .Where(p => p.Name == "Gas boiler" || p.Name == "Oil boiler")
                        .OrderBy(p => p.ProductionCost / p.MaxHeat)
                        .ToList();
                case OptimizationType.Scenario2:
                    return SortProductionUnitsForScenario2(productionUnits, electricalPrice);
                default:
                    throw new ArgumentException("Invalid optimization type");
            }
        }

        private List<EnergyData> SortProductionUnitsForScenario2(IEnumerable<EnergyData> productionUnits , EnergyData electrialPrice)
        {
            double highPriceThreshold = 1000;
            double lowPriceThreshold = 650;
            double electricityPrice = electrialPrice.ElectricityPrice;

            if (electricityPrice < lowPriceThreshold)
            {
                // Always choose the Electric Boiler
                return productionUnits
                    .OrderByDescending(p => p.Name == "Electric boiler")
                    .ThenBy(p => p.ProductionCost)
                    .ToList();
            }
            else if (electricityPrice > highPriceThreshold)
            {
                // Use the Gas Motor first, then add the motors with the lowest price
                return productionUnits
                    .OrderByDescending(p => p.Name == "Gas motor")
                    .ThenBy(p => p.ProductionCost)
                    .ToList();
            }
            else
            {
                // Use the lowest price motors
                return productionUnits.OrderBy(p => p.ProductionCost).ToList();
            }
        }

        private double CalculateCost(EnergyData unit, double heatProduced)
        {
            return heatProduced * unit.ProductionCost;
        }

        private double CalculateEmissions(EnergyData unit, double heatProduced)
        {
            return unit.CO2Emission * heatProduced;
        }
    }
}
