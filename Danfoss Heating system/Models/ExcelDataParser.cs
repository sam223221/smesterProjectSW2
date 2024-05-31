using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace Danfoss_Heating_system.Models
{

    public class ExcelDataParser
    {
        private string filePath;
        public ExcelDataParser(string exfilePath)
        {
            string relativePath = @"..\..\..\Assets\data.xlsx";
            this.filePath = Path.GetFullPath(relativePath);
        }

        public List<EnergyData> ParseQuotes()
        {
            var quotesList = new List<EnergyData>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet("SDM");  // Making sure we are always using the right sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows
                CultureInfo DanishInfo = new CultureInfo("da-DK");

                foreach (var row in rows)
                {

                    if (row.RowNumber() <= 3) continue;  // Skipping header rows

                    // Assuming the quotes and their authors are in columns 10 and 11 respectively
                    var quote = row.Cell(12).GetValue<string>();
                    var quoteAuthor = row.Cell(13).GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(quote) && !string.IsNullOrWhiteSpace(quoteAuthor)) // Assuming there are 15 rows of quotes starting from row 4
                    {
                        quotesList.Add(new EnergyData
                        {
                            Quotes = quote,
                            quoteAuther = quoteAuthor
                        });
                    }
                    else if (!string.IsNullOrWhiteSpace(quote))
                    {
                        quotesList.Add(new EnergyData
                        {
                            Quotes = quote,
                            quoteAuther = "Anonymous Auther"
                        });
                    }
                }
            }

            return quotesList;
        }

        public List<EnergyData> UserInfo()
        {
            var list = new List<EnergyData>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet("SDM");  // Making sure we are always using the right sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows
                CultureInfo DanishInfo = new CultureInfo("da-DK");

                foreach (var row in rows)
                {

                    if (row.RowNumber() == 1) continue; // skip the first Row

                    var userID = row.Cell(15).GetValue<string>(); // Gather User ID to check 
                    var userPassword = row.Cell(16).GetValue<string>(); // Gather Password to check
                    var userRole = row.Cell(17).GetValue<string>(); // Gather the Role to direct the UI

                    if (!string.IsNullOrWhiteSpace(userID) && !string.IsNullOrWhiteSpace(userPassword))
                    {
                        list.Add(new EnergyData
                        {
                            UserID = userID,
                            UserPassword = userPassword,
                            UserRole = userRole
                        });
                    }

                }


            }
            return list;
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
                var worksheet = workbook.Worksheet("SDM");  // Making sure we are always using the right sheet
                var rows = worksheet.RangeUsed().RowsUsed();  // Skip header rows
                CultureInfo DanishInfo = new CultureInfo("da-DK");



                foreach (var row in rows)
                {

                    if (row.RowNumber() <= 3) continue;  // Skipping header rows

                    // Extracting values as strings directly from the cells
                    var timeFromStringWinter = row.Cell(1).GetValue<DateTime>();   // Winter TimeFrom
                    var timeToStringWinter = row.Cell(2).GetValue<DateTime>();   // Winter TimeTo
                    double.TryParse(row.Cell(3).GetValue<String>(), NumberStyles.Any, DanishInfo, out heatDemandStringWinter);          // Winter HeatDemand                                                                            
                    double.TryParse(row.Cell(4).GetValue<String>(), NumberStyles.Any, DanishInfo, out electricityPriceStringWinter);    // Winter ElectricityPrice


                    energyDataList.Add(new EnergyData
                    {
                        TimeFrom = timeFromStringWinter,
                        TimeTo = timeToStringWinter,
                        HeatDemand = heatDemandStringWinter,
                        ElectricityPrice = electricityPriceStringWinter,
                        Season = "Winter"
                    });

                    var timeFromStringSummer = row.Cell(6).GetValue<DateTime>();   // Summer TimeFrom
                    var timeToStringSummer = row.Cell(7).GetValue<DateTime>();   // Summer TimeTo
                    double.TryParse(row.Cell(8).GetValue<String>(), NumberStyles.Any, DanishInfo, out heatDemandStringSummer);          // Winter HeatDemand
                    double.TryParse(row.Cell(9).GetValue<String>(), NumberStyles.Any, DanishInfo, out electricityPriceStringSummer);    // Winter ElectricityPrice

                    energyDataList.Add(new EnergyData
                    {
                        TimeFrom = timeFromStringSummer,
                        TimeTo = timeToStringSummer,
                        HeatDemand = heatDemandStringSummer,
                        ElectricityPrice = electricityPriceStringSummer,
                        Season = "Summer"
                    });
                }

            }

            return energyDataList;
        }

        public List<EnergyData> ParserProductionUnits()
        {
            var productionUnitList = new List<EnergyData>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet("SDM");
                var rows = worksheet.RangeUsed().RowsUsed();
                CultureInfo DanishInfo = new CultureInfo("da-DK");

                foreach (var row in rows)
                {
                    if (row.RowNumber() <= 3) continue; //Skipping header rows
                    var name = row.Cell(22).GetValue<String>();
                    double.TryParse(row.Cell(23).GetValue<String>(), NumberStyles.Any, DanishInfo, out double maxHeat);
                    double.TryParse(row.Cell(24).GetValue<String>(), NumberStyles.Any, DanishInfo, out double maxElectricity);
                    double.TryParse(row.Cell(25).GetValue<String>(), NumberStyles.Any, DanishInfo, out double productionCost);
                    double.TryParse(row.Cell(26).GetValue<String>(), NumberStyles.Any, DanishInfo, out double co2Emissions);
                    double.TryParse(row.Cell(27).GetValue<String>(), NumberStyles.Any, DanishInfo, out double gasConsumption);

                    if (!string.IsNullOrEmpty(name))
                    {
                        productionUnitList.Add(new EnergyData
                        {
                            Name = name,
                            MaxHeat = maxHeat,
                            MaxElectricity = maxElectricity,
                            ProductionCost = productionCost,
                            CO2Emission = co2Emissions,
                            GasConsumption = gasConsumption,
                        });
                    }
                }
            }
            return productionUnitList;
        }
    }

}
