using System;

namespace Danfoss_Heating_system.Models
{
    public class EnergyData
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
        public string? Season { get; set; }
        public string? Quotes { get; set; }
        public string? quoteAuther { get; set; }
        public string? UserID { get; set; }
        public string? UserPassword { get; set; }
        public string? UserRole { get; set; }


        public string Name { get; set; }
        public double MaxHeat { get; set; }
        public double MaxElectricity { get; set; }
        public double ProductionCost { get; set; }
        public double CO2Emission { get; set; }
        public double GasConsumption { get; set; }

        public int PercentageUsage { get; set; }

        public string DisplayText => $"On the Day: {TimeFrom:D} From {TimeFrom:HH:mm} To: {TimeTo:HH:mm}, The Heat demand: {HeatDemand.ToString("0.000")}, Price: {ElectricityPrice:C}";
        public string? DisplayQuotes => Quotes;
        public string? DisplayQuoteAuthor => quoteAuther;
    }

    public class OperationResult
    {
        public DateTime Date { get; set; }
        public string? UnitsUsed { get; set; }
        public double TotalCost { get; set; }
        public double CO2Emissions { get; set; }
        public double HeatDemand { get; internal set; }
        public double ElectricityPrice { get; internal set; }
    }



}
