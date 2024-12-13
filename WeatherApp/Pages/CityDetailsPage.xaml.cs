namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class CityDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;
    private int _cityId;
    private bool _loginPageDisplayed = false;

    private WeatherData _weatherData;

    public CityDetailsPage(ApiService apiService, FavoritesService favoritesService, 
                           IRestService restService, IValidator validator, 
                           WeatherData weatherData, int cityId, string cityName)
    {
        InitializeComponent();
        _apiService = apiService;
        _favoritesService = favoritesService;
        _restService = restService;
        _validator = validator;
        _weatherData = weatherData;
        Title = cityName ?? "City Details";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCityDetails(_cityId);
        UpdateFavoriteButton();
    }

    private async Task<City?> GetCityDetails(int cityId)
    {
        var (cityDetails, errorMessage) = await _apiService.GetCityDetails(cityId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (cityDetails == null)
        {
            await DisplayAlert("Error", errorMessage ?? "Unable to get city", "OK");
            return null;
        }

        if (cityDetails != null)
        {
            lblCityName.Text = cityDetails.Name;
            lblCityLat.Text = cityDetails.Lat.ToString();
            lblCityLon.Text = cityDetails.Lon.ToString();
            lblCountry.Text = cityDetails.Country;
            lblState.Text = cityDetails.State;
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "Unable to get city details", "OK");
            return null;
        }

        return cityDetails;
    }

    private async void UpdateFavoriteButton()
    {
        var favoriteExists = await _favoritesService.ReadAsync(_cityId);

        if (favoriteExists != null)
        {
            BtnFavoritesImage.Source = "heartfill";
        }
        else
        {
            BtnFavoritesImage.Source = "heart";
        }
    }

    private async void BtnFavoritesImage_Clicked(object sender, EventArgs e)
    {
        try
        {
            var favoriteExists = await _favoritesService.ReadAsync(_cityId);

            if (favoriteExists != null)
            {
                await _favoritesService.DeleteAsync(favoriteExists);
            }
            else
            {
                var favoriteCity = new FavoriteCity
                {
                    CityId = _cityId,
                    Name = lblCityName.Text,
                    Lat = Convert.ToDouble(lblCityLat.Text),
                    Lon = Convert.ToDouble(lblCityLon.Text),
                    Country = lblCountry.Text,
                    State = lblState.Text,
                    IsFavorite = true,
                };

                await _favoritesService.CreateAsync(favoriteCity);
            }

            UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _favoritesService, 
                                                 _restService, _validator, _weatherData));
    }

}