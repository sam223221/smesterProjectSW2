using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{
    internal class OPTProp
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
        public List<string>? MotorUsage { get; set; }
        public string ProductionPrice { get; set; }
        public double CO2Emission { get; set; }
        public string ElectricalBoiler { get; set; }


    }
}
