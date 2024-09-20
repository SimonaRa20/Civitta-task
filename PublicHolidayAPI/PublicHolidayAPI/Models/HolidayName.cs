namespace PublicHolidayAPI.Models
{
    public class HolidayName
    {
        public int Id { get; set; }
        public string Lang { get; set; }
        public string Text { get; set; }

        public int HolidayId { get; set; }
        public Holiday Holiday { get; set; }
    }
}
