using DarkSkyApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
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
        public async Task<IActionResult> Record(double? tempC, double? humidity, bool test = false)
        {
            //Don't bother if we have no temp data
            if (tempC.HasValue || humidity.HasValue)
            {
                double? outsideTemp = null;

                try
                {
                    var client = new DarkSkyService(_darkSky.ApiKey);
                    var forecast = await client.GetWeatherDataAsync(_darkSky.Latitude, _darkSky.Longitude, Unit.CA);
                    outsideTemp = forecast.Currently.Temperature;
                }
                catch (HttpRequestException)
                {
                    //Unable to call the DarkSkyApi, just leave outsideTemp as null.
                }

                //Docker is running in a linux container on UTC time, so we need to get it in EST.
                var tzi = TZConvert.GetTimeZoneInfo("America/New_York");
                var estDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, tzi.Id);

                Console.WriteLine(estDateTime + " - Temp: " + tempC + " C, Humidity: " + humidity + "%, Outside: " + outsideTemp + " C");

                //If we're testing, don't save to the db.
                if (!test)
                {
                    var record = new Record() { SensorHumidity = humidity, SensorTemp = tempC, Timestamp = estDateTime };
                    if (outsideTemp.HasValue)
                    {
                        record.OutsideTemp = outsideTemp.Value;
                    }
                    _thermometerRepo.Insert(record);
                }
            }

            return Ok();
        }
    }
}