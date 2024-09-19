using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.Services;

namespace PublicHolidayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IKayaposoftApiService _holidayService;

        public HolidayController(IKayaposoftApiService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _holidayService.GetCountriesAsync();
            return Ok(countries);
        }
    }
}
