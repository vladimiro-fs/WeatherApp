namespace WeatherApp.Pages;

using System.Collections.ObjectModel;
using WeatherApp.Models;
using WeatherApp.Services;

public partial class HomePage : ContentPage
{
    private readonly IRestService _restService;
    private readonly CitiesService _citiesService;
    private List<City> _matchingCities;
    private ObservableCollection<FavoriteCity>
        FavoriteCities = new ObservableCollection<FavoriteCity>();

    public HomePage(IRestService restService, CitiesService citiesService)
	{
		InitializeComponent();
        _restService = restService;
        _citiesService = citiesService;       
        _matchingCities = new List<City>();
        
        LoadFavoriteCities();
    }

    private async void LoadFavoriteCities()
    {
        try
        {
            FavoriteCities.Clear();

            var cities = await _citiesService.ReadAllAsync();

            foreach (var city in cities)
            {
                FavoriteCities.Add(city);
            }

            FavoritesPicker.ItemsSource = FavoriteCities;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load favorites: {ex.Message}", "OK");
        }
    }

    private async void OnCityEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string input = e.NewTextValue;

        if (string.IsNullOrWhiteSpace(input))
        {
            _matchingCities.Clear();           
            BtnForecast.IsEnabled = false;
            return;
        }

        // Fetch city suggestions using RestService
        var matchingCities = await FetchCitySuggestionsAsync(input);
        _matchingCities = matchingCities; 
        
        BtnForecast.IsEnabled = _matchingCities.Any();
    }
   
    private async Task<List<City>> FetchCitySuggestionsAsync(string input)
    {
        try
        {
            var cities = await _restService.GetCitiesAsync(input);
                     
            var matchingCities = cities.Select(c => new City
            {
                Name = c.Name,
                ApiId = c.ApiId,
                Lat = c.Lat,
                Lon = c.Lon,
                Country = c.Country,
                State = c.State ?? string.Empty,  
            }).ToList();

            return matchingCities;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch city suggestions: {ex.Message}", "OK");
            return new List<City>();
        }
    }

    private async void BtnForecast_Clicked(object sender, EventArgs e)
    {
        string cityName = lblCityEntry.Text;

        if (string.IsNullOrWhiteSpace(cityName))
        {
            await DisplayAlert("Error", "Please enter a city name.", "OK");
            return;
        }

        if (_matchingCities == null || !_matchingCities.Any())
        {
            await DisplayAlert("Error", "No city suggestions available. Please try again later.", "OK");
            return;
        }

        // Find the selected city from the suggestions
        var selectedCity = _matchingCities.FirstOrDefault(c =>
        {
            bool isMatch = !string.IsNullOrEmpty(c.Name) && c.Name.Trim().Equals(cityName.Trim(), StringComparison.OrdinalIgnoreCase);           
            return isMatch;
        });

        if (selectedCity == null)
        {
            await DisplayAlert("Error", "City not found in suggestions. Please select a valid city.", "OK");
            return;
        }

        try
        {
            // Fetch weather data for the selected city
            var weatherData = await _restService.GetWeatherDataAsync(selectedCity.Lat, selectedCity.Lon);

            // Navigate to WeatherPage
            await Navigation.PushAsync(new WeatherPage(weatherData, _citiesService));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch weather data: {ex.Message}", "OK");
        }
    }

    private void BtnFavoritesForecast_Clicked(object sender, EventArgs e)
    {

    }
}