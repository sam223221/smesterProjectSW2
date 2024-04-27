using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{
    public class SortingTheDaysInExcelParser
    {
        public Dictionary<DateTime, List<EnergyData>> SortDataByDate(List<EnergyData> allData)
        {
            var sortedData = allData
                .GroupBy(data => data.TimeFrom.Date) // Use Date property to group by date ignoring time
                .ToDictionary(group => group.Key, group => group.ToList());

            return sortedData;
        }

    }
}
