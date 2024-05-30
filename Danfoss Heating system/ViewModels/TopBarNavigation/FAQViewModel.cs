using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels.TopBarNavigation
{
    partial class FAQViewModel : ViewModelBase
    {
        private int _selectedQuestionIndex;
        public int SelectedQuestionIndex
        {
            get => _selectedQuestionIndex;
            set
            {
                if (_selectedQuestionIndex != value)
                {
                    _selectedQuestionIndex = value;
                    OnPropertyChanged(nameof(SelectedQuestionIndex));
                    OnPropertyChanged(nameof(CurrentAnswer));
                }
            }
        }

        public ObservableCollection<string> Questions { get; set; }
        public ObservableCollection<string> Answers { get; set; }

        public string CurrentAnswer => Answers.Count > SelectedQuestionIndex ? Answers[SelectedQuestionIndex] : string.Empty;

        public FAQViewModel()
        {
            Questions = new ObservableCollection<string>
            {
                "How does the Graph Optimizer work?", 
                "How does the Live Auto Optimizer work?",
                "How does the Live Manual Optimizer work?", 
                "What is the goal of this application?", 
                "Who can benefit from this application?", 
                "How can it be used to reduce costs?",
                "Can this app contribute to the enviroment?", 
                "What technologies were used for development?", 
            };
            Answers = new ObservableCollection<string>
            {
                "It takes in the desired outdated heating data ​and outputs graphs based on client's preferences.",
                "It calculates a predicted value based ​on past days and turns on and off motors ​based on the calculated value.", 
                "It is a fully manual solution which works based on the client's preferences. It allows you to select between motors at all times.",
                "The goal of this application is to provide a solution for optimizing heat production and streamlinining the thermal distribiuton in a specific area.",  
                "Companies specialized in the heating industry that seek for a solution to optimize their results.", 
                "Cost reduction can be accomplished by optimizing in relation to lowest cost which can be done using one of the Optimizers.", 
                "Yes, optimizers can also be set to work in relation to lowest CO2 emission.", 
                "The app is written in AvaloniaUI and C#.",
            };
        }
    }
}