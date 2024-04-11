using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.Models
{

/*
    input:

    two dates : from and to

    outputs:

    list with : 
    1. which motors are being use each day and for how long
    2. optinal input for chosen eco friendly or electrisity

    output a list of things
    1. date and hour
    2. which motors are being used
    3. what the demand is
    4. production price
    5. CO2 emissions by production
    6. electrical boiler
    

    every calcualtion needs to check if it makes more sence to use electric boiler
 */



    internal class OPT
    {
        ExcelDataParser excelDataParser;


        public OPT(string filePath)
        {
            excelDataParser = new ExcelDataParser(filePath);
        }

        // GetOPTData is the main method that will be called from the ViewModel
        public List<OPTProp> GetOPTData(DateTime from, DateTime to) => MainOPT(from,to);



        // MainOPT is the main method that will be called from the GetOPTData method
        private List<OPTProp> MainOPT(DateTime from, DateTime to)
        {
            var optimizationData = new List<OPTProp>();


            return optimizationData;
        }



    }
}
