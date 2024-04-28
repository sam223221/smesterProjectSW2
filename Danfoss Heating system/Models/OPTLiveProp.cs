using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{
    internal class OPTLiveProp
    {

        public EnergyData data;
        public bool isUnitEnabled { get; set; }
        public double PredictedHeatDemand { get; set; }
        public bool IsMachineEnabled { get; set; } = false;
        public double remainingHeatDemand { get; set; }
        public double? usingCO2Emission { get; set; }
        public double usingHeatDemand { get; set; }


        public OPTLiveProp(EnergyData data)
        {
            this.data = data;
        }




    }
}
