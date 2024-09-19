using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.Interfaces
{
    public interface IKayaposoftApiService
    {
        Task<List<Country>> GetCountriesAsync();
    }
}
