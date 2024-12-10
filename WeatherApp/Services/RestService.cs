namespace WeatherApp.Services
{
    using System.Text.Json;
    using WeatherApp.Models;

    public class RestService : IRestService
    {
        private readonly HttpClient _httpClient;

        JsonSerializerOptions _serializerOptions;

        public RestService(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<List<City>> GetCitiesAsync(string cityName) 
        {
            string endpoint = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={Constants.OpenWeatherMapAPIKey}";

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode) 
            { 
                using var responseStream = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync<List<City>>(responseStream, _serializerOptions);
            }
            else
                throw new Exception("Failed to fetch cities");            
        }

        public async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude) 
        {            
            string endpoint = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={Constants.OpenWeatherMapAPIKey}";

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode) 
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync<WeatherData>(responseStream, _serializerOptions);
            }
            else
                throw new Exception("Failed to fetch weather data");           
        }
    }
}
