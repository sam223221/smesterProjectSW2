using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reactive.Disposables;


namespace Danfoss_Heating_system.Models
{

    public class ExcelDataParser
    {
        private string filePath;
        public ExcelDataParser(string exfilePath) 
        { 
            filePath = exfilePath;
        }

        public List<EnergyData> ParseQuotes()
        {
            var quotesList = new List<EnergyData>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);  // Assuming data is in the first sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows
                CultureInfo DanishInfo = new CultureInfo("da-DK");

                foreach (var row in rows)
                {

                    if (row.RowNumber() <= 3) continue;  // Skipping header rows

                    // Assuming the quotes and their authors are in columns 10 and 11 respectively
                    var quote = row.Cell(12).GetValue<string>();
                    var quoteAuthor = row.Cell(13).GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(quote) && row.RowNumber() <= 18) // Assuming there are 15 rows of quotes starting from row 4
                    {
                        quotesList.Add(new EnergyData
                        {
                            Quotes = quote,
                            quoteAuther = quoteAuthor
                        });
                    }
                }
            } 

            return quotesList;
        }

        public List<EnergyData> ParserEnergyData()
        {

            var energyDataList = new List<EnergyData>();
             
            double electricityPriceStringWinter;    // Winter ElectricityPrice
            double electricityPriceStringSummer;    // Summer ElectricityPrice
            double heatDemandStringSummer;          // Summer HeatDemand
            double heatDemandStringWinter;          // Winter HeatDemand

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);  // Assuming data is in the first sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows
                CultureInfo DanishInfo = new CultureInfo("da-DK");



                foreach (var row in rows)
                {

                    if (row.RowNumber() <= 3) continue;  // Skipping header rows

                    // Extracting values as strings directly from the cells
                    var timeFromStringWinter            = row.Cell(1).GetValue<DateTime>();   // Winter TimeFrom
                    var timeToStringWinter              = row.Cell(2).GetValue<DateTime>();   // Winter TimeTo
                    double.TryParse(row.Cell(3).GetValue<String>(), NumberStyles.Any, DanishInfo, out heatDemandStringWinter);          // Winter HeatDemand                                                                            
                    double.TryParse(row.Cell(4).GetValue<String>(), NumberStyles.Any, DanishInfo, out electricityPriceStringWinter);    // Winter HeatDemand

                    energyDataList.Add(new EnergyData
                    {
                        TimeFrom            = timeFromStringWinter,
                        TimeTo              = timeToStringWinter,
                        HeatDemand          = heatDemandStringWinter,
                        ElectricityPrice    = electricityPriceStringWinter,
                        Season              = "Winter"
                    });

                    var timeFromStringSummer            = row.Cell(6).GetValue<DateTime>();   // Summer TimeFrom
                    var timeToStringSummer              = row.Cell(7).GetValue<DateTime>();   // Summer TimeTo
                    double.TryParse(row.Cell(8).GetValue<String>(),NumberStyles.Any, DanishInfo, out heatDemandStringSummer);          // Winter HeatDemand
                    double.TryParse(row.Cell(9).GetValue<String>(),NumberStyles.Any, DanishInfo, out electricityPriceStringSummer);    // Winter HeatDemand

                    energyDataList.Add(new EnergyData
                    {
                        TimeFrom            = timeFromStringSummer,
                        TimeTo              = timeToStringSummer,
                        HeatDemand          = heatDemandStringSummer,
                        ElectricityPrice    = electricityPriceStringSummer,
                        Season              = "Summer"
                    });
                }

            }

            return energyDataList;
        }
    }

}
