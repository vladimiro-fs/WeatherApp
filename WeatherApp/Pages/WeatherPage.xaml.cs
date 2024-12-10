namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;

public partial class WeatherPage : ContentPage
{
    private readonly WeatherData _weatherData;
    private readonly CitiesService _citiesService;
    private bool _isFavorite;


    public WeatherPage(WeatherData weatherData, CitiesService citiesService)
	{
		InitializeComponent();
        _weatherData = weatherData;
        _citiesService = citiesService;
        BindingContext = weatherData;

        InitializeFavoriteStatusAsync();
	}

    private async void InitializeFavoriteStatusAsync()
    {
        var favorite = await _citiesService.ReadAsync(_weatherData.ApiId);
        _isFavorite = favorite != null;
        FavoriteSwitch.IsToggled = _isFavorite;
    }

    private async void OnFavoriteToggled(object sender, ToggledEventArgs e)
    {
        _isFavorite = e.Value;

        var favoriteCity = new FavoriteCity
        {
            ApiId = _weatherData.ApiId,
            Name = _weatherData.Name
        };

        if (_isFavorite)
        {
            await _citiesService.CreateAsync(favoriteCity);
        }
        else
        {
            await _citiesService.DeleteAsync(favoriteCity);
        }
    }
}