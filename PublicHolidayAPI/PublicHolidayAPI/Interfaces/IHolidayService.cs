using PublicHolidayAPI.ResponseModels;

namespace PublicHolidayAPI.Interfaces
{
    public interface IHolidayService
    {
        Task<List<CountriesResponse>> GetCountriesAsync();
        Task<List<MonthHolidaysResponse>> GetHolidaysAsync(string countryCode, int year);
        Task<bool> IsPublicHolidayAsync(string countryCode, DateTime date);
        Task<bool> IsWorkDayAsync(string countryCode, DateTime date);
        Task<DayStatusResponse> GetSpecificDayStatusAsync(string countryCode, DateTime date);
        Task<int> GetMaxConsecutiveFreeDaysAsync(string countryCode, int year);
    }
}
