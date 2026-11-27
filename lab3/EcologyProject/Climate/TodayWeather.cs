namespace EcologyProject;

public class TodayWeather
{
    public double Temperature { get; }
    public double Precipitation { get; }
    public double CloudCover { get; }

    public TodayWeather(AreaClimate climate)
    {
        ArgumentNullException.ThrowIfNull(climate);
        Temperature = climate.minTemperature + Random.Shared.NextDouble() * (climate.maxTemperature - climate.minTemperature);
        Precipitation = climate.minPrecipitation + Random.Shared.NextDouble() * (climate.maxPrecipitation - climate.minPrecipitation);
        CloudCover = climate.minCloudCover + Random.Shared.NextDouble() * (climate.maxCloudCover - climate.minCloudCover);
    }

    public void ShowCurrentWeather()
    {
        Console.WriteLine($"Temperature: {Temperature:F1} °C");
        Console.WriteLine($"Precipitation: {Precipitation:F1} %");
        Console.WriteLine($"Cloud cover: {CloudCover:F1} %");
    }
}
