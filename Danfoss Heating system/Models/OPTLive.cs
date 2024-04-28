using Avalonia.Controls.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models;


/* 
input: 
1.newst date: datetime format  


outputs: list:      
1. which motors need to be trued on     
2. timer for each hour so 10 min before the whole hour it needs to trun on a motor     
3. current production     
4. lowest cost solution     
5. most eco friendly solution     
6. total cost     
7. current emmitions  manuale
*/




internal class OPTLive
{
        ExcelDataParser ExcelDataParser;
        

        Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> optimalizationDataStatus = new Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)>();

        public OPTLive(string filePath)
        {
            this.ExcelDataParser = new ExcelDataParser(filePath);
        }


        public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> UsingMachines(double predictedHeatDemand)
        {
            var productionUnits = ExcelDataParser.ParserProductionUnits();
         
             

            foreach (var unit in productionUnits) 
            {
                var localOPTProp = new OPTLiveProp(unit);


                if(unit.MaxHeat <= predictedHeatDemand)
                {
                    localOPTProp.isUnitEnabled          = true;
                    predictedHeatDemand                 -= unit.MaxHeat;
                    localOPTProp.usingHeatDemand        = unit.MaxHeat;
                    localOPTProp.usingCO2Emission       = unit.CO2Emission * localOPTProp.usingHeatDemand;
                    
                    optimalizationDataStatus.Add(unit.Name, (localOPTProp.isUnitEnabled, localOPTProp));
                }

               else
               {
                    if(predictedHeatDemand > 0)
                    {
                        localOPTProp.isUnitEnabled = true;
                        localOPTProp.usingHeatDemand = unit.MaxHeat - localOPTProp.remainingHeatDemand;
                        localOPTProp.usingCO2Emission = localOPTProp.usingHeatDemand * unit.CO2Emission;

                        optimalizationDataStatus.Add(unit.Name, (localOPTProp.isUnitEnabled, localOPTProp));
                    }
                    else
                    {
                        localOPTProp.isUnitEnabled = false;
                    }
               }
            }
            return optimalizationDataStatus;
        }

    public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> LowestCost()
    {
        return optimalizationDataStatus.OrderBy(unit => unit.Value.OPTProp.data.ProductionCost)
                 .ToDictionary(unit => unit.Key, unit => unit.Value);    
    }


    public Dictionary<string, (bool isEnabled, OPTLiveProp OPTProp)> EcoFriendly()
    {
        return optimalizationDataStatus.OrderBy(unit => unit.Value.OPTProp.data.CO2Emission)
            .ToDictionary(unit => unit.Key, unit => unit.Value);
    }


    public double ProductionCostPerHour()
    {
            // Calculate how much heat i am using from each unit and  multiply this per its cost
         double TotalOneHourCost = 0;
        foreach (var unit in optimalizationDataStatus)
        {

                TotalOneHourCost += unit.Value.OPTProp.usingHeatDemand * unit.Value.OPTProp.data.ProductionCost;
        }
            return TotalOneHourCost;
    }

        public double CO2EmmitionsPerHour()
        {
            double TotalCO2Emmition = 0;
            foreach(var unit in optimalizationDataStatus)
            {
                TotalCO2Emmition += unit.Value.OPTProp.usingCO2Emission ?? 0;
            }
            return TotalCO2Emmition;
        }
}
