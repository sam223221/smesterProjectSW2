using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{
    public class SortingTheDaysInExcelParserProps
    {
        public required string date { get; set; }
        public required List<EnergyData> data { get; set; }
    }

}
