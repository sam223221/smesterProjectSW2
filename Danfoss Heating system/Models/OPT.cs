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



    // cheapestHeatDemandUsage is a method that will be called from the MainOPT method
    private List<OPTProp> cheapestHeatDemandUsage(double heatdemand, List<EnergyData> motorUnits)
    {
        var list = new List<OPTProp>();
        var units = new List<EnergyData>();
        double productionPrice = 0;

        // Loop through the motor units and check if the heat demand is greater than 0
        for (int i = 0; i < motorUnits.Count && heatdemand > 0; i++)
        {
            productionPrice += motorUnits[i].ProductionCost;
            heatdemand -= motorUnits[i].MaxHeat;
            units.Add(motorUnits[i]);
        }
        // Add the data to the list
        list.Add(new OPTProp
        {
            ProductionPrice = productionPrice,
            MotorUsage = units
        });


        return list;
    }

}

