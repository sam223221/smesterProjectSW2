using System.Collections.Generic;

namespace Danfoss_Heating_system.Models
{
    public class SortingTheDaysInExcelParserProps
    {
        public string date { get; set; }
        public List<EnergyData> data { get; set; }
        public string background { get; set; }
    }

}
