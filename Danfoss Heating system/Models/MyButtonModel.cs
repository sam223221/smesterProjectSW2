using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Danfoss_Heating_system.ViewModels;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Danfoss_Heating_system.Models
{
    public class MyButtonModel 
    {

        public ReactiveCommand<string, Unit> example { get; }
        public string Name { get; set; }
        public ICommand Command { get; set; }

        public MyButtonModel(string name, ReactiveCommand<string, Unit> example)
        {
            this.Name = name;
            this.example = example;
        }
    }
}
