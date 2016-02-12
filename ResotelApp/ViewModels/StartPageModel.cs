using System;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using ResotelApp.ViewModels.Utils;
using System.Windows.Data;
using System.Windows.Markup;

namespace ResotelApp.ViewModels
{
    class StartPageModel
    {
        public List<String> RequiredFields { get; private set; }
        public MarkupExtension FontWeightConverter { get; set; }

        public StartPageModel()
        {
            RequiredFields = new List<string> {
                    "Nom", "Prenom"
            };
            
        }

        
    }
}
