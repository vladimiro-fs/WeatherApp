namespace WeatherApp.Services
{
    using WeatherApp.Models;

    public interface IRestService
    {
        Task<List<City>> GetCitiesAsync(string cityName);

        Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude);
    }
}
