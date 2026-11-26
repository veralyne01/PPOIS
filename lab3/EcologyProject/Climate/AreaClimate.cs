using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class AreaClimate
    {
        public double minTemperature { get; private set; }
        public double maxTemperature { get; private set; }
        public double minPrecipitation { get; private set; }
        public double maxPrecipitation { get; private set; }
        public double minCloudCover { get; private set; }
        public double maxCloudCover { get; private set; }
        public AreaClimate(double minTemperature, double maxTemperature, double minPrecipitation, double maxPrecipitation, double minCloudCover, double maxCloudCover)
        {
            this.minTemperature = minTemperature;
            this.maxTemperature = maxTemperature;
            this.minPrecipitation = minPrecipitation;
            this.maxPrecipitation = maxPrecipitation;
            this.minCloudCover = minCloudCover;
            this.maxCloudCover = maxCloudCover;
        }
        public void ShowClimateInfo()
        {
            Console.WriteLine($"Temperature condition: {minTemperature}-{maxTemperature} °C");
            Console.WriteLine($"Precipitation condition: {minPrecipitation}-{maxPrecipitation} %");
            Console.WriteLine($"Cloud cover: {minCloudCover}-{maxCloudCover} %");
        }
    }
}
