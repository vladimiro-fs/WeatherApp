namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;

public partial class WeatherPage : ContentPage
{
    private readonly WeatherData _weatherData;
    private readonly FavoritesService _favoritesService;
    private bool _isFavorite;

    public WeatherPage(WeatherData weatherData, FavoritesService favoritesService)
	{
		InitializeComponent();
        _weatherData = weatherData;
        _favoritesService = favoritesService;
        BindingContext = weatherData;
	}

    protected override void OnAppearing() 
    {
        base.OnAppearing();
        InitializeFavoriteStatusAsync();
    }

    private async void InitializeFavoriteStatusAsync()
    {
        var favorite = await _favoritesService.ReadAsync(_weatherData.ApiId);
        _isFavorite = favorite != null;
        FavoriteSwitch.IsToggled = _isFavorite;
    }

    private async void FavoriteSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        _isFavorite = e.Value;

        var favoriteCity = new FavoriteCity
        {
            ApiId = _weatherData.ApiId,
            Name = _weatherData.Name
        };

        if (_isFavorite)
        {
            await _favoritesService.CreateAsync(favoriteCity);
        }
        else
        {
            await _favoritesService.DeleteAsync(favoriteCity);
        }
    }
}