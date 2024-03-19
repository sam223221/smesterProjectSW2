using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;


namespace Danfoss_Heating_system.Models
{

    public class ExcelDataParser
    {
        public List<EnergyData> ParseExcel(string filePath)
        {
            var energyDataList = new List<EnergyData>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);  // Assuming data is in the first sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows

                foreach (var row in rows)
                {
                    if (row.RowNumber() <= 3) continue;  // Skipping header rows

                    
                    var timeFrom = DateTime.Parse(row.Cell(2).GetValue<string>());          // Winter TimeFrom
                    var timeTo = DateTime.Parse(row.Cell(3).GetValue<string>());            // Winter TimeTo
                    var heatDemand = double.Parse(row.Cell(4).GetValue<string>());          // Winter HeatDemand
                    var electricityPrice = double.Parse(row.Cell(5).GetValue<string>());    // Winter ElectricityPrice

                    energyDataList.Add(new EnergyData { TimeFrom = timeFrom, TimeTo = timeTo, HeatDemand = heatDemand, ElectricityPrice = electricityPrice, Season = "Winter" });

                    timeFrom = DateTime.Parse(row.Cell(7).GetValue<string>());          // Summer TimeFrom
                    timeTo = DateTime.Parse(row.Cell(8).GetValue<string>());            // Summer TimeTo
                    heatDemand = double.Parse(row.Cell(9).GetValue<string>());          // Summer HeatDemand
                    electricityPrice = double.Parse(row.Cell(10).GetValue<string>());   // Summer ElectricityPrice

                    energyDataList.Add(new EnergyData { TimeFrom = timeFrom, TimeTo = timeTo, HeatDemand = heatDemand, ElectricityPrice = electricityPrice, Season = "Summer" });
                }
            }

            return energyDataList;
        }
    }

}
