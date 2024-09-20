using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.Interfaces
{
    public interface IKayaposoftApiService
    {
        Task<List<Country>> GetCountriesAsync();
        Task<List<Holiday>> GetHolidaysAsync(string countryCode, int year);
    }
}
