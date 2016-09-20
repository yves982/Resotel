namespace ResotelApp.Models
{
    public static class RoomKindExtensions
    {
        /// <summary>
        /// Extracts the BedKind out of a RoomKind
        /// </summary>
        /// <param name="kind">the source RoomKind</param>
        /// <returns>the associated BedKind</returns>
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
