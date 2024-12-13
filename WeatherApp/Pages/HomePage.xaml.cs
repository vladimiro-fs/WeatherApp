namespace WeatherApp.Pages;

using System.Collections.ObjectModel;
using WeatherApp.Models;
using WeatherApp.Services;

public partial class HomePage : ContentPage
{
    private readonly IRestService _restService;
    private readonly FavoritesService _favoritesService;
    private List<City> _matchingCities;
    private ObservableCollection<FavoriteCity> FavoriteCities; 

    public HomePage(IRestService restService, FavoritesService favoritesService)
	{
		InitializeComponent();
        lblWelcome.Text = "Hello, " + Preferences.Get("username", string.Empty) + " ,where should we take you?";
        _restService = restService;
        _favoritesService = favoritesService;       
        _matchingCities = new List<City>();
        FavoriteCities = new ObservableCollection<FavoriteCity>();
        BtnForecast.IsEnabled = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAndDisplayFavoriteCitiesAsync();
    }

    private async Task LoadAndDisplayFavoriteCitiesAsync()
    {
        try
        {
            FavoriteCities.Clear();

            var favoriteCities = await _favoritesService.ReadAllAsync();

            if (favoriteCities == null || favoriteCities.Count == 0)
            {
                lblWarning.IsVisible = true;
                cvCities.ItemsSource = null;
            }
            else
            {
                foreach (var city in favoriteCities)
                {
                    FavoriteCities.Add(city);
                }

                lblWarning.IsVisible = false;
                cvCities.ItemsSource = FavoriteCities;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load favorite cities: {ex.Message}", "OK");
        }
    }

    private async Task<List<City>> GetMatchingCityAsync(string input)
    {
        try
        {
            return await _restService.GetCitiesAsync(input) ?? new List<City>();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch matching cities: {ex.Message}", "OK");
            return new List<City>();
        }
    }

    private async void CityEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string input = e.NewTextValue;

        if (string.IsNullOrEmpty(input))
        {
            _matchingCities.Clear();
            BtnForecast.IsEnabled = false;
            return;
        }

        _matchingCities = await GetMatchingCityAsync(input);
        BtnForecast.IsEnabled = _matchingCities.Any();
    }

    private async void BtnForecast_Clicked(object sender, EventArgs e)
    {
        FavoriteCity selectedFavoriteCity = cvCities.SelectedItem as FavoriteCity;

        if (!string.IsNullOrWhiteSpace(CityEntry.Text))
        {
            var matchingCity = _matchingCities.FirstOrDefault(c =>
                string.Equals(c.Name?.Trim(), CityEntry.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (matchingCity == null)
            {
                await DisplayAlert("Error", "No matching city found.", "OK");
                return;
            }

            try
            {
                var weatherData = await _restService.GetWeatherDataAsync(matchingCity.Lat, matchingCity.Lon);
                await Navigation.PushAsync(new WeatherPage(weatherData, _favoritesService));
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to fetch weather data: {ex.Message}", "OK");
            }
        }
        else if (selectedFavoriteCity != null)
        {
            try
            {
                var weatherData = await _restService.GetWeatherDataAsync(selectedFavoriteCity.Lat, selectedFavoriteCity.Lon);
                await Navigation.PushAsync(new WeatherPage(weatherData, _favoritesService));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to fetch weather data: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Please enter a city name or select a favorite city.", "OK");
        }
    }

    private void cvCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedFavoriteCity = e.CurrentSelection.FirstOrDefault() as FavoriteCity;

        if (selectedFavoriteCity == null)
            return;

        BtnForecast.IsEnabled = selectedFavoriteCity != null;
    }
}