using Microsoft.AspNetCore.Mvc;
using Moq;
using PublicHolidayAPI.Controllers;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Models;
using PublicHolidayAPI.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicHolidayAPI.Tests
{
    public class HolidayControllerUnitTests
    {
        [Fact]
        public async Task GetCountries_ReturnsOkResult_WithListOfCountries()
        {
            // setup
            var mockHolidayService = new Mock<IHolidayService>();
            mockHolidayService
                .Setup(service => service.GetCountriesAsync())
                .ReturnsAsync(new List<CountriesResponse>
                {
                new CountriesResponse { CountryCode = "US", FullName = "United States" }
                });

            var controller = new HolidayController(mockHolidayService.Object);

            // action
            var result = await controller.GetCountries();

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var countries = Assert.IsType<List<CountriesResponse>>(okResult.Value);
            Assert.Single(countries);
            Assert.Equal("US", countries[0].CountryCode);
        }

        [Fact]
        public async Task GetHolidays_ReturnsHolidays_ForGivenCountryAndYear()
        {
            // setup
            var mockHolidayService = new Mock<IHolidayService>();
            mockHolidayService
                .Setup(service => service.GetHolidaysAsync("US", 2023))
                .ReturnsAsync(new List<MonthHolidaysResponse>
                {
            new MonthHolidaysResponse
            {
                Month = 12,
                Holidays = new List<HolidayResponse>
                {
                    new HolidayResponse
                    {
                        Name = "Christmas",
                        Date = new DateTime(2023, 12, 25),
                        Type = Contracts.HolidayTypes.public_holiday,
                    }
                }
            }
                });

            var controller = new HolidayController(mockHolidayService.Object);

            // action
            var result = await controller.GetHolidays("US", 2023);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var holidays = Assert.IsType<List<MonthHolidaysResponse>>(okResult.Value);
            Assert.Single(holidays);
            Assert.Equal(12, holidays[0].Month);
            Assert.Single(holidays[0].Holidays);
            Assert.Equal("Christmas", holidays[0].Holidays[0].Name);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsHoliday_WhenDateIsHoliday()
        {
            // Arrange
            var mockHolidayService = new Mock<IHolidayService>();
            mockHolidayService
                .Setup(service => service.GetSpecificDayStatusAsync("US", new DateTime(2023, 1, 1)))
                .ReturnsAsync(new DayStatusResponse { Status = "Holiday", Date = new DateDetails { Day = 1, Month = 1, Year = 2023 } });

            var controller = new HolidayController(mockHolidayService.Object);

            // Act
            var result = await controller.GetSpecificDayStatus("US", new DateTime(2023, 1, 1));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var status = Assert.IsType<DayStatusResponse>(okResult.Value);
            Assert.Equal("Holiday", status.Status);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsWeekend_WhenDateIsWeekend()
        {
            // Arrange
            var mockHolidayService = new Mock<IHolidayService>();
            mockHolidayService
                .Setup(service => service.GetSpecificDayStatusAsync("US", new DateTime(2023, 9, 23)))
                .ReturnsAsync(new DayStatusResponse { Status = "Weekend", Date = new DateDetails { Day = 23, Month = 9, Year = 2023 } });

            var controller = new HolidayController(mockHolidayService.Object);

            // Act
            var result = await controller.GetSpecificDayStatus("US", new DateTime(2023, 9, 23));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var status = Assert.IsType<DayStatusResponse>(okResult.Value);
            Assert.Equal("Weekend", status.Status);
        }

        [Fact]
        public async Task GetSpecificDayStatusAsync_ReturnsWorkDay_WhenDateIsWorkDay()
        {
            // Arrange
            var mockHolidayService = new Mock<IHolidayService>();
            mockHolidayService
                .Setup(service => service.GetSpecificDayStatusAsync("US", new DateTime(2023, 9, 25)))
                .ReturnsAsync(new DayStatusResponse { Status = "WorkDay", Date = new DateDetails { Day = 25, Month = 9, Year = 2023 } });

            var controller = new HolidayController(mockHolidayService.Object);

            // Act
            var result = await controller.GetSpecificDayStatus("US", new DateTime(2023, 9, 25));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var status = Assert.IsType<DayStatusResponse>(okResult.Value);
            Assert.Equal("WorkDay", status.Status);
        }
    }
}
