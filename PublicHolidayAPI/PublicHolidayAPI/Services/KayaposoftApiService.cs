using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PublicHolidayAPI.Contracts;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.ResponseModels;

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
                var link = Consts.BaseEndpoint + "/json/v2.0/?action=getSupportedCountries";
                var response = await _httpClient.GetAsync(link);
                var countriesJson = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<Country>>(countriesJson);

                _context.Countries.AddRange(countries);
                await _context.SaveChangesAsync();
            }

            return await _context.Countries.ToListAsync();
        }

        public async Task<List<Holiday>> GetHolidaysAsync(string countryCode, int year)
        {
            if (!_context.Holidays.Any(h => h.Country.CountryCode == countryCode && h.Date.Year == year))
            {
                var link = Consts.BaseEndpoint + $"/json/v2.0/?action=getHolidaysForYear&year={year}&country={countryCode}";
                var response = await _httpClient.GetAsync(link);
                var responseString = await response.Content.ReadAsStringAsync();

                if (responseString.Contains("\"error\""))
                {
                    var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                    Console.WriteLine($"API error: {error["error"]}");
                    return new List<Holiday>();
                }

                try
                {
                    var holidaysApiResponse = JsonConvert.DeserializeObject<List<HolidayApiResponse>>(responseString);
                    var country = _context.Countries.FirstOrDefault(c => c.CountryCode == countryCode);

                    if (holidaysApiResponse != null && country != null)
                    {
                        var holidays = holidaysApiResponse.Select(h => new Holiday
                        {
                            CountryId = country.Id,
                            Name = country.CountryCode,
                            Date = new DateTime(h.Date.Year, h.Date.Month, h.Date.Day),
                            Type = Enum.Parse<HolidayTypes>(h.HolidayType),

                            Names = h.Name.Select(n => new HolidayName
                            {
                                Lang = n.Lang,
                                Text = n.Text
                            }).ToList(),

                            Notes = h.Note?.Select(n => new HolidayNote
                            {
                                Lang = n.Lang,
                                Text = n.Text
                            }).ToList() ?? new List<HolidayNote>(),

                            Flags = h.Flags ?? new List<string>(),

                            ObservedOn = h.ObservedOn != null ? new DateTime(h.ObservedOn.Year, h.ObservedOn.Month, h.ObservedOn.Day) : (DateTime?)null
                        }).ToList();

                        _context.Holidays.AddRange(holidays);
                        await _context.SaveChangesAsync();

                        return holidays;
                    }
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    return new List<Holiday>();
                }
            }

            return await _context.Holidays
                                 .Where(h => h.Country.CountryCode == countryCode && h.Date.Year == year)
                                 .Include(h => h.Names)
                                 .Include(h => h.Notes)
                                 .ToListAsync();
        }

    }
}
