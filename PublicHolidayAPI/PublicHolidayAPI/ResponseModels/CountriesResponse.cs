using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.ResponseModels
{
    public class CountriesResponse
    {
        public string CountryCode { get; set; }
        public List<string> Regions { get; set; }
        public List<string> HolidayTypes { get; set; }
        public string FullName { get; set; }
        public DateDetails FromDate { get; set; }
        public DateDetails ToDate { get; set; }
    }
}
