using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{
    public class EnergyData
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
        public string? Season { get; set; }
        public string? Quotes {  get; set; }
        public string? quoteAuther { get; set; }
        public string? UserID { get; set; }
        public string? UserPassword { get; set; }
        public string? UserRole { get; set; }


<<<<<<< HEAD
        public string Name { get; set; }
        public double MaxHeat { get; set; }
        public double MaxElectricity { get; set; }
        public double ProductionCost { get; set; }
        public double CO2Emission { get; set; }
        public double GasConsumption { get; set; }
=======
       public string Name { get; set; }
       public double MaxHeat { get; set; }
       public double? MaxElectricity { get; set; }
        public double ProductionCost { get; set; }
       public double? CO2Emission { get; set; }
        public double? GasConsumption { get; set; }
>>>>>>> LiveOptimizer


        public string DisplayText => $"On the Day: {TimeFrom:D} From {TimeFrom:HH:mm} To: {TimeTo:HH:mm}, The Heat demand: {HeatDemand.ToString("0.000")}, Price: {ElectricityPrice:C}";
        public string? DisplayQuotes => Quotes;
        public string? DisplayQuoteAuthor => quoteAuther;    
    }

}
