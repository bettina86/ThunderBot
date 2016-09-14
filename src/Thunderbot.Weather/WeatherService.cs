using GeocodeSharp.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderBot.Weather
{
    public class WeatherService
    {
        public async Task<string> GetCurrentConditionsAsync(WeatherLocation city)
        {
            var forecast = new ForecastIOPortable.ForecastApi("8703082d49387d351564670982b3bc7c");
            var weather = await forecast.GetWeatherDataAsync(city.Latitude, city.Longitude);

            var currentTemp = weather.Currently.ApparentTemperature;
            var currentConditions = weather.Currently.Summary;

            return $"Right now in {city.LocationName} it's {currentConditions} with a temperature of {currentTemp}";
        }

        public async Task<WeatherLocation> FindLocationCoordinatesAsync(string locationName)
        {
            var gc = new GeocodeClient();
            var gcResponse = await gc.GeocodeAddress(locationName);
            var gcResult = gcResponse.Results.FirstOrDefault();

            if (gcResult == null)
                return null;

            return new WeatherLocation()
            {
                Latitude = gcResult.Geometry.Location.Latitude,
                Longitude = gcResult.Geometry.Location.Longitude,
                LocationName = locationName
            };
        }

        public async Task<string> GetConditionsForLocationAtTimeAsync(WeatherLocation loc, DateTime value)
        {
            var forecast = new ForecastIOPortable.ForecastApi("8703082d49387d351564670982b3bc7c");
            var weather = await forecast.GetWeatherDataAsync(loc.Latitude, loc.Longitude);

			weather.Daily.Days.Where(d => d.)

            throw new NotImplementedException();
        }
    }
}
