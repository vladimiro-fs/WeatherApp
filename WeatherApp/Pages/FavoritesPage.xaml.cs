namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;

    private WeatherData _weatherData;

    public FavoritesPage(ApiService apiService, FavoritesService favoritesService, 
                         IRestService restService, IValidator validator, 
                         WeatherData weatherData)
    {
        InitializeComponent();
        _apiService = apiService;
        _favoritesService = favoritesService;
        _restService = restService;
        _validator = validator;
        _weatherData = weatherData;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteCities();
    }

    private async Task GetFavoriteCities()
    {
        try
        {
            var favoriteCities = await _favoritesService.ReadAllAsync();

            if (favoriteCities == null || favoriteCities.Count == 0)
            {
                cvCities.ItemsSource = null;
                lblWarning.IsVisible = true;
            }
            else
            {
                cvCities.ItemsSource = favoriteCities;
                lblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void cvCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteCity;

        if (currentSelection == null)
        {
            return;
        }

        Navigation.PushAsync(new CityDetailsPage(_apiService, _favoritesService, _restService, 
                                                 _validator, _weatherData, currentSelection.CityId, currentSelection.Name!));

        ((CollectionView)sender).SelectedItem = null;
    }
}