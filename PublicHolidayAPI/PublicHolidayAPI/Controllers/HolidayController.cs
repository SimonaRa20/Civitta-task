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

        /// <summary>
        /// Retrieves the list of countries.
        /// </summary>
        /// <returns></returns>
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching countries.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the holidays for a specific country and year.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("holidays/{countryCode}/{year}")]
        public async Task<IActionResult> GetHolidays(string countryCode, int year)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching holidays.", error = ex.Message });
            }
        }

        /// <summary>
        /// Returns flag if given day is public holiday in given country.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("isPublicHoliday/{countryCode}/{date}")]
        public async Task<IActionResult> IsPublicHoliday(string countryCode, DateTime date)
        {
            try
            {
                if (date == default || string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest("Invalid request parameters.");
                }

                var holidays = await _context.Holidays
                    .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                    .ToListAsync();

                if (!holidays.Any())
                {
                    holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
                }

                var isPublicHoliday = holidays.Any(h => h.Date.Date == date.Date);

                return Ok(new { isPublicHoliday });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking public holiday.", error = ex.Message });
            }
        }

        /// <summary>
        /// Returns flag if given day is work day in given country.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("isWorkDay/{countryCode}/{date}")]
        public async Task<IActionResult> IsWorkDay(string countryCode, DateTime date)
        {
            try
            {
                if (date == default || string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest("Invalid request parameters.");
                }

                var holidays = await _context.Holidays
                    .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                    .ToListAsync();

                if (!holidays.Any())
                {
                    holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
                }

                var isPublicHoliday = holidays.Any(h => h.Date.Date == date.Date);
                var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                var isWorkDay = !(isPublicHoliday || isWeekend);

                return Ok(new { isWorkDay });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking work day.", error = ex.Message });
            }
        }

        /// <summary>
        /// Returns information about specific day status.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("specific-day-status/{countryCode}/{date}")]
        public async Task<IActionResult> GetSpecificDayStatus(string countryCode, DateTime date)
        {
            try
            {
                if (date == default || string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest("Invalid request parameters.");
                }

                var holidays = await _context.Holidays
                    .Where(h => h.Country.CountryCode == countryCode && h.Date.Date == date.Date)
                    .ToListAsync();

                if (!holidays.Any())
                {
                    holidays = await _kayaposoftApiService.GetHolidaysAsync(countryCode, date.Year);
                }

                var status = string.Empty;
                var isHoliday = holidays.Any();
                Holiday selectDate = holidays.Where(x => x.Date == date).FirstOrDefault();

                if (selectDate == null)
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        status = "Weekend";
                    }
                    else
                    {
                        status = "WorkDay";
                    }
                }
                if (isHoliday)
                {
                    status = "Holiday";
                }

                var response = new DayStatusResponse
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving specific day status.", error = ex.Message });
            }
        }

        /// <summary>
        /// Returns the maximum number of free days in a row.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("max-consecutive-free-days/{countryCode}/{year}")]
        public async Task<IActionResult> GetMaxConsecutiveFreeDays(string countryCode, int year)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countryCode) || year <= 0)
                {
                    return BadRequest("Invalid request parameters.");
                }

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

                return Ok(new { maxConsecutiveFreeDays = maxStreak });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating consecutive free days.", error = ex.Message });
            }
        }
    }
}
