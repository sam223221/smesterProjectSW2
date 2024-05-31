using ReactiveUI;
using System.Reactive;

namespace Danfoss_Heating_system.Models
{
    public class GraphProps
    {

        public double HeatDemand { get; set; }
        public string Heating { get; set; }
        public string? date { get; set; }
        public string? background { get; set; }
        public ReactiveCommand<string, Unit>? ChosenPiler { get; }

        public GraphProps(double heatDemand, string? date, ReactiveCommand<string, Unit>? chosenPiler, string background, string heating)
        {
            HeatDemand = heatDemand;
            this.date = date;
            ChosenPiler = chosenPiler;
            this.background = background;
            Heating = heating;
        }

    }
}
