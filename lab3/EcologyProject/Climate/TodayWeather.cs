using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    class TodayWeather
    {
        private double temperature;
        private double precipitation;
        private double cloudCover;
        public TodayWeather(AreaClimate climate)
        {
            temperature = climate.minTemperature + Random.Shared.NextDouble() * (climate.maxTemperature - climate.minTemperature);
            precipitation = climate.minPrecipitation + Random.Shared.NextDouble() * (climate.maxPrecipitation - climate.minPrecipitation);
            cloudCover = climate.minCloudCover + Random.Shared.NextDouble() * (climate.maxCloudCover - climate.minCloudCover);
        }
    
        public void ShowCurrentWeather()
        {
            Console.WriteLine($"Temperature: {temperature} °C");
            Console.WriteLine($"Precipitation: {precipitation} %");
            Console.WriteLine($"Temperature: {cloudCover} %");
        }
    }
}
