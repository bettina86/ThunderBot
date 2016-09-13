using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderBot.WeatherFull
{
    public class WeatherService
    {
        public async Task<string> GetCurrentConditions(string city)
        {
            var gc = new GeocodeSharp.Google.GeocodeClient();
            var gcResponse = await gc.GeocodeAddress(city);

            var gcResult = gcResponse.Results.FirstOrDefault();

            if (gcResult == null)
                return "C'mon give me a city that I know!";

            var theLocation = gcResult.FormattedAddress;
            var loc = gcResult.Geometry.Location;

            var forecast = new ForecastIOPortable.ForecastApi("8703082d49387d351564670982b3bc7c");
            var weather = await forecast.GetWeatherDataAsync(loc.Latitude, loc.Longitude);

            var currentTemp = weather.Currently.ApparentTemperature;
            var currentConditions = weather.Currently.Summary;

            return $"Right now in {theLocation} it's {currentConditions} with a temperature of {currentTemp}";
        }

    }
}
