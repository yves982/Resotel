namespace ResotelApp.Models
{
    public class Option
    {
        public int OptionId { get; set; }
        public string Label { get; set; }
        public bool IsRoomRelated { get; set; }
        public int BasePrice { get; set; }
    }
}