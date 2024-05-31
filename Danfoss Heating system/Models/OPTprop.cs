using System;
using System.Collections.Generic;

namespace Danfoss_Heating_system.Models
{
    internal class OPTProp
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
        public List<EnergyData> MotorUsage { get; set; }
        public double ProductionPrice { get; set; }
        public double CO2Emission { get; set; }
        public string ElectricalBoiler { get; set; }


    }
}
