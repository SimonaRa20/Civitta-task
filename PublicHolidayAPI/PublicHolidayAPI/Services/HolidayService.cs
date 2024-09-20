using Microsoft.EntityFrameworkCore;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.ResponseModels;

namespace PublicHolidayAPI.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IKayaposoftApiService _kayaposoftApiService;
        private readonly HolidayDbContext _context;

        public HolidayService(IKayaposoftApiService kayaposoftApiService, HolidayDbContext context)
        {
            _kayaposoftApiService = kayaposoftApiService;
            _context = context;
        }

        public async Task<List<CountriesResponse>> GetCountriesAsync()
        {
            List<Country> countries = await _context.Countries.ToListAsync();
            if (!countries.Any())
            {
                countries = await _kayaposoftApiService.GetCountriesAsync();
            }

            return countries.Select(country => new CountriesResponse
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
        }

        public async Task<List<MonthHolidaysResponse>> GetHolidaysAsync(string countryCode, int year)
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

            return groupedByMonth;
        }

        public async Task<bool> IsPublicHolidayAsync(string countryCode, DateTime date)
        {
            var holidays = await _context.Holidays
                .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                .ToListAsync();

            if (!holidays.Any())
            {
                holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
            }

            return holidays.Any(h => h.Date.Date == date.Date);
        }

        public async Task<bool> IsWorkDayAsync(string countryCode, DateTime date)
        {
            var holidays = await _context.Holidays
                .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                .ToListAsync();

            if (!holidays.Any())
            {
                holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
            }

            var isPublicHoliday = holidays.Any(h => h.Date.Date == date.Date);
            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

            return !(isPublicHoliday || isWeekend);
        }

        public async Task<DayStatusResponse> GetSpecificDayStatusAsync(string countryCode, DateTime date)
        {
            var holidays = await _context.Holidays
                .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                .ToListAsync();

            if (!holidays.Any())
            {
                holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
            }

            var status = holidays.Any() ? "Holiday" : date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? "Weekend" : "WorkDay";

            return new DayStatusResponse
            {
                Date = new DateDetails
                {
                    Day = date.Day,
                    Month = date.Month,
                    Year = date.Year,
                    DayOfWeek = (int)date.DayOfWeek + 1
                },
                Status = status
            };
        }

        public async Task<int> GetMaxConsecutiveFreeDaysAsync(string countryCode, int year)
        {
            var holidays = await _context.Holidays
                .Where(h => h.Country.CountryCode == countryCode && h.Date.Year == year)
                .ToListAsync();

            if (!holidays.Any())
            {
                holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, year);
            }

            var freeDays = new HashSet<DateTime>(holidays.Select(h => h.Date));

            var firstDayOfYear = new DateTime(year, 1, 1);
            var lastDayOfYear = new DateTime(year, 12, 31);
            for (var date = firstDayOfYear; date <= lastDayOfYear; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    freeDays.Add(date);
                }
            }

            var maxStreak = 0;
            var currentStreak = 0;

            var sortedFreeDays = freeDays.OrderBy(d => d).ToList();
            for (int i = 0; i < sortedFreeDays.Count; i++)
            {
                if (i == 0 || (sortedFreeDays[i] - sortedFreeDays[i - 1]).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    maxStreak = Math.Max(maxStreak, currentStreak);
                    currentStreak = 1;
                }
            }

            maxStreak = Math.Max(maxStreak, currentStreak);
            return maxStreak;
        }
    }
}
