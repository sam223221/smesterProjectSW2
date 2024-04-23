using Avalonia.Controls;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system
{
    public class MyButtonModel
    {
        public string ImageSource { get; set; } = "avares://Danfoss_Heating_system/Assets/Images/development.png";
        public double Width { get; set; } = 50;
        public string Background { get; set; } = "Blue";

        public double ImageWidth { get; set; } = 40;
        public double ImageHeight { get; set; } = 40;
    }
}
