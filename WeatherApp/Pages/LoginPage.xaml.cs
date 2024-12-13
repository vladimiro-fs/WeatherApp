namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;

    private WeatherData _weatherData;

    public LoginPage(ApiService apiService, FavoritesService favoritesService,
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

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "The email field can't be empty", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Error", "The password field can't be empty.", "Cancel");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
            Application.Current!.MainPage = new AppShell(_apiService, _favoritesService, 
                                                         _restService, _validator, _weatherData);

        else
            await DisplayAlert("Error", "Something went wrong", "Cancel");
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _favoritesService, 
                                                    _restService, _validator, _weatherData));
    }
}