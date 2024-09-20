using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.ResponseModels
{
    public class HolidayApiResponse
    {
        public DateDetails Date { get; set; }
        public List<HolidayNameApiResponse> Name { get; set; }
        public List<HolidayNoteApiResponse> Note { get; set; }
        public List<string> Flags { get; set; }
        public string HolidayType { get; set; }
        public DateDetails ObservedOn { get; set; }
    }
}
