namespace Danfoss_Heating_system.Models
{
    public class OPTLiveProp
    {
        public EnergyData data { get; set; }
        public string? NameOfUnit { get; set; }
        public bool isUnitEnabled { get; set; }
        public double PredictedHeatDemand { get; set; }
        public double remainingHeatDemand { get; set; }
        public double usingCO2Emission { get; set; }
        public double usingHeatDemand { get; set; }
        public string stateOfUnit { get; set; }
        public string operationOfUnit { get; set; }
        public double operationCost { get; set; }
        public int UsageInPercentPerHour { get; set; }

        public OPTLiveProp(EnergyData data)
        {
            this.data = data;
        }
        public OPTLiveProp() { }
    }
}
