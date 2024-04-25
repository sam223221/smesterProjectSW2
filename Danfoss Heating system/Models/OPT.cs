using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models;


/*
input:

two dates : from and to

outputs:

list with : 
1. which motors are being use each day and for how long
2. optimal input for chosen eco friendly or electricity

output a list of things
1. date and hour
2. which motors are being used
3. what the demand is
4. production price
5. CO2 emissions by production
6. electrical boiler
    

every calculation needs to check if it makes more sense to use electric boiler
*/



internal class OPT
{
    ExcelDataParser excelDataParser;
    string filePath;

    public OPT(string filePath)
    {
        this.filePath = filePath;
    }


    // GetOPTData is the main method that will be called from the ViewModel
    public Dictionary<string,OPTProp> GetOPTData(DateTime from, DateTime to) => MainOPT(from,to);



    // MainOPT is the main method that will be called from the GetOPTData method
    private Dictionary<string,OPTProp> MainOPT(DateTime from, DateTime to)
    {
        // Get the data from the excel file
        var parser = new ExcelDataParser(filePath);
        var excelDataParser = parser.ParserEnergyData();
        var motorData = parser.ParserProductionUnits();

        // Create a list to store the data
        var optimizationData = new Dictionary<string, OPTProp>();

        // Loop through the data
        foreach (var item in excelDataParser)
        {
            // Check if the date is within the range
            if (item.TimeFrom >= from && item.TimeTo <= to)
            {

                /*---- adding the best co2 friendly ----*/
                



                /* ---- Adding the cheapest solution to the ---- */
                var motorsUsed = cheapestHeatDemandUsage(item.HeatDemand, motorData);

                optimizationData.Add("Cheapest solution", new OPTProp
                {
                    TimeFrom = item.TimeFrom,
                    TimeTo = item.TimeTo,
                    HeatDemand = item.HeatDemand,
                    ElectricityPrice = item.ElectricityPrice,
                    MotorUsage = motorsUsed[0].MotorUsage,
                    ProductionPrice = motorsUsed[0].ProductionPrice
                });
            }
        }
           

        return optimizationData;
    }

    private List<OPTProp> bestCO2Friendly(double heatdemand, List<EnergyData> motorUnits)
    {
        var list = new List<OPTProp>();
        var units = new List<EnergyData>();
        double productionPrice = 0;
        double co2Emission = 0;

        // Loop through the motor units and check if the heat demand is greater than 0
        for (int i = 0; i < motorUnits.Count && heatdemand > 0; i++)
        {
            productionPrice += motorUnits[i].ProductionCost;
            co2Emission += motorUnits[i].CO2Emission;
            heatdemand -= motorUnits[i].MaxHeat;
            units.Add(motorUnits[i]);
        }
        // Add the data to the list
        list.Add(new OPTProp
        {
            ProductionPrice = productionPrice,
            MotorUsage = units,
            CO2Emission = co2Emission
        });

        return list;
    }

    // cheapestHeatDemandUsage is a method that will be called from the MainOPT method
    private List<OPTProp> cheapestHeatDemandUsage(double heatdemand, List<EnergyData> motorUnits)
    {
        var list = new List<OPTProp>();
        var units = new List<EnergyData>();
        double productionPrice = 0;
        double co2Emission = 0;

        // Loop through the motor units and check if the heat demand is greater than 0
        for (int i = 0; i < motorUnits.Count && heatdemand > 0; i++)
        {
            productionPrice += motorUnits[i].ProductionCost;
            co2Emission += motorUnits[i].CO2Emission;
            heatdemand -= motorUnits[i].MaxHeat;
            units.Add(motorUnits[i]);
        }
        // Add the data to the list
        list.Add(new OPTProp
        {
            ProductionPrice = productionPrice,
            MotorUsage = units,
            CO2Emission = co2Emission
        });


        return list;
    }

    private double predictHeatDemand(int hourDemandCell, bool winter, string excelFilePath)
    {
        //Predicted heat demand
        double predHeatDemand = 0;
        //Offset of cells in the excel sheet
        int offset = 4;

        string cellOne = "D4";
        string cellTwo = "D4";
        string cellThr = "D4";

        //Reference to the excel sheet
        var workbook = new XLWorkbook(excelFilePath);


        //Setting the references to the excel cells
        if (winter)
        {
            cellOne = "D" + (hourDemandCell - 24).ToString();
            cellTwo = "D" + (hourDemandCell - 48).ToString();
            cellThr = "D" + (hourDemandCell - 72).ToString();
        } else
        {
            cellOne = "I" + (hourDemandCell - 24).ToString();
            cellTwo = "I" + (hourDemandCell - 48).ToString();
            cellThr = "I" + (hourDemandCell - 72).ToString();
        }
    

        //This if statement makes the method return 0 as a heat demand if the data is insufficient
        if (hourDemandCell >= 24 + offset)
        {
            //Getting the data from the excel cells
            double demand24 = Convert.ToDouble((string)workbook.Cell(cellOne).Value);
            double demand48 = Convert.ToDouble((string)workbook.Cell(cellTwo).Value);
            double demand72 = Convert.ToDouble((string)workbook.Cell(cellThr).Value);

            // This is here so that if we don't have enough heat demand data for past days, the program won't die
            if (hourDemandCell >= 72 + offset)
            {
                predHeatDemand = demand24 + demand48 + demand72 / 3;
            }
            else if (hourDemandCell >= 48 + offset)
            {
                predHeatDemand = demand24 + demand48 / 2;
            }
            else
            {
                predHeatDemand = demand24;
            }
        }

        return predHeatDemand;
    }

}

