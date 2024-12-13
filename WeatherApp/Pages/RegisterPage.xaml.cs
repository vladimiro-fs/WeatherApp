namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;

    private WeatherData _weatherData;

    public RegisterPage(ApiService apiService, FavoritesService favoritesService,
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

    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntName.Text, EntEmail.Text, EntPhoneNumber.Text, EntPassword.Text))
        {
            var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text, EntPhoneNumber.Text, EntPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Success", "Your account was successfully created", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _favoritesService, 
                                                         _restService, _validator, _weatherData));
            }
            else
                await DisplayAlert("Error", "Something went wrong", "Cancel");
        }
        else
        {
            string errorMessage = "";

            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PhoneNumberError != null ? $"{_validator.PhoneNumberError}" : "";
            errorMessage += _validator.PasswordError != null ? $"{_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMessage, "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _favoritesService,
                                                 _restService, _validator, _weatherData)); 
    }
}