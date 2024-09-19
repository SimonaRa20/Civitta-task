using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;

namespace PublicHolidayAPI.Services
{
    public class KayaposoftApiService : IKayaposoftApiService
    {
        private readonly HttpClient _httpClient;
        private readonly HolidayDbContext _context;

        public KayaposoftApiService(HttpClient httpClient, HolidayDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                var response = await _httpClient.GetAsync("https://kayaposoft.com/enrico/json/v2.0/?action=getSupportedCountries");
                var countriesJson = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<Country>>(countriesJson);

                _context.Countries.AddRange(countries);
                await _context.SaveChangesAsync();
            }

            return await _context.Countries.ToListAsync();
        }
    }
}
