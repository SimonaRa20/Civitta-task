using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.ResponseModels;
using PublicHolidayAPI.Services;

namespace PublicHolidayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IKayaposoftApiService _kayaposoftApiService;
        private readonly HolidayDbContext _context;

        public HolidayController(IKayaposoftApiService holidayService, HolidayDbContext context)
        {
            _kayaposoftApiService = holidayService;
            _context = context;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            List<Country> countries = await _context.Countries.ToListAsync();
            if (!countries.Any())
            {
                countries = await _kayaposoftApiService.GetCountriesAsync();
            }

            var countryDtos = countries.Select(country => new CountriesResponse
            {
                CountryCode = country.CountryCode,
                Regions = country.Regions,
                HolidayTypes = country.HolidayTypes.Select(ht => ht.ToString()).ToList(),
                FullName = country.FullName,
                FromDate = new DateDetails
                {
                    Day = country.FromDate.Day,
                    Month = country.FromDate.Month,
                    Year = country.FromDate.Year
                },
                ToDate = new DateDetails
                {
                    Day = country.ToDate.Day,
                    Month = country.ToDate.Month,
                    Year = country.ToDate.Year
                }
            }).ToList();

            return Ok(countryDtos);
        }

        [HttpGet("holidays/{countryCode}/{year}")]
        public async Task<IActionResult> GetHolidays(string countryCode, int year)
        {
            var holidays = _context.Holidays
                .Where(h => h.Country.CountryCode == countryCode && h.Date.Year == year)
                .ToList();

            if (!holidays.Any())
            {
                holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, year);
            }

            var groupedByMonth = holidays
                .GroupBy(h => h.Date.Month)
                .Select(g => new MonthHolidaysResponse
                {
                    Month = g.Key,
                    Holidays = g.Select(h => new HolidayResponse
                    {
                        Name = h.Name,
                        Date = h.Date,
                        Names = h.Names.Select(n => new HolidayNameResponse
                        {
                            Lang = n.Lang,
                            Text = n.Text
                        }).ToList(),
                        Flags = h.Flags,
                        Type = h.Type
                    }).ToList()
                })
                .ToList();

            return Ok(groupedByMonth);
        }

    }
}
