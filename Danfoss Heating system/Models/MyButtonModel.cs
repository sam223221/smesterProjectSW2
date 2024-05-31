using ReactiveUI;
using System.Reactive;

namespace Danfoss_Heating_system.Models
{
    public class MyButtonModel
    {

        public ReactiveCommand<string, Unit> example { get; }
        public string Name { get; set; }

        public MyButtonModel(string name, ReactiveCommand<string, Unit> example)
        {
            this.Name = name;
            this.example = example;
        }
    }
}
