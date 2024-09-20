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
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
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
                var countries = await _holidayService.GetCountriesAsync();
                return Ok(countries);
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
                var holidays = await _holidayService.GetHolidaysAsync(countryCode, year);
                return Ok(holidays);
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
                var isPublicHoliday = await _holidayService.IsPublicHolidayAsync(countryCode, date);
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
                var isWorkDay = await _holidayService.IsWorkDayAsync(countryCode, date);
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
                var status = await _holidayService.GetSpecificDayStatusAsync(countryCode, date);
                return Ok(status);
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
                var maxConsecutiveFreeDays = await _holidayService.GetMaxConsecutiveFreeDaysAsync(countryCode, year);
                return Ok(new { maxConsecutiveFreeDays });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating consecutive free days.", error = ex.Message });
            }
        }
    }
}
