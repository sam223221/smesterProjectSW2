﻿using System;
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
        public string Season { get; set; }
        public string Quotes {  get; set; }


        public string DisplayText => $"On the Day: {TimeFrom:D} From {TimeFrom:HH:mm} To: {TimeTo:HH:mm}, The Heat demand: {HeatDemand.ToString("0.000")}, Price: {ElectricityPrice:C}";
    }

}
