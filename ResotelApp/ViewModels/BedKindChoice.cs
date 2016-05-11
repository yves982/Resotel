using ResotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class BedKindChoice
    {
        public BedKind Kind { get; set; }
        public int AvailableCount { get; set; }
        public int ChoosenCount { get; set; }
    }
}
