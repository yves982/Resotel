using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.Models
{
    public static class RoomKindExtensions
    {
        public static BedKind ToBedKind(this RoomKind kind)
        {
            BedKind bedKind = BedKind.Simple;
            switch(kind)
            {
                case RoomKind.Simple:
                    bedKind = BedKind.Simple;
                    break;
                case RoomKind.Double:
                    bedKind = BedKind.Double;
                    break;
                case RoomKind.DoubleWithBaby:
                    bedKind = BedKind.DoubleWithBaby;
                    break;
                default:
                    bedKind = BedKind.Simple;
                    break; 
            }
            return bedKind;
        }
    }
}
