using DarkSkyApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Thermometer.Models;
using Thermometer.Models.AppSettings;
using Thermometer.Repository;
using TimeZoneConverter;

namespace Thermometer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ThermometerController : Controller
    {
        private readonly DarkSky _darkSky;
        private readonly IThermometerRepo _thermometerRepo;

        public ThermometerController(IOptions<DarkSky> darkSky, IThermometerRepo thermometerRepo)
        {
            _darkSky = darkSky.Value;
            _thermometerRepo = thermometerRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Record(double tempC, double humidity)
        {
            Console.WriteLine(DateTime.Now + " Temp: " + tempC + " C, Humidity: " + humidity + "%");

            var client = new DarkSkyService(_darkSky.ApiKey);
            //My location
            var forecast = await client.GetWeatherDataAsync(43.704045, -79.400547, Unit.CA);
            var outsideTemp = forecast.Currently.Temperature;

            //Docker is running in a linux container on UTC time, so we need to get it in EST.
            var tzi = TZConvert.GetTimeZoneInfo("America/New_York");
            var estDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, tzi.Id);

            var record = new Record() { SensorHumidity = humidity, SensorTemp = tempC, OutsideTemp = outsideTemp, Timestamp = estDateTime };
            _thermometerRepo.Insert(record);

            return Ok();
        }
    }
}