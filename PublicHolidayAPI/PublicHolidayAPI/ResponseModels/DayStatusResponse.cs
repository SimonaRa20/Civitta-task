using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.ResponseModels
{
    public class DayStatusResponse
    {
        public DateDetails Date { get; set; }
        public string Status { get; set; }
    }
}
