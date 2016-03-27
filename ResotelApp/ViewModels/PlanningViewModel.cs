using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;
using System.Resources;

namespace ResotelApp.ViewModels
{
    class PlanningViewModel
    {

        
        public string Title { get; private set; }
        public DateRange TimeFrame { get; set; }

        public PlanningViewModel()
        {
            Title = TranslationHandler.GetString("PlanningTitle");
        }
    }
}
