using PublicHolidayAPI.Contracts;

namespace PublicHolidayAPI.ResponseModels
{
    public class HolidayResponse
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<HolidayNameResponse> Names { get; set; } = new List<HolidayNameResponse>();
        public List<string> Flags { get; set; } = new List<string>();
        public HolidayTypes Type { get; set; }
    }
}
