namespace PublicHolidayAPI.ResponseModels
{
    public class MonthHolidaysResponse
    {
        public int Month { get; set; }
        public List<HolidayResponse> Holidays { get; set; } = new List<HolidayResponse>();
    }
}
