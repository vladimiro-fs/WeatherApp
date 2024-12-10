namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly CitiesService _citiesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;

    private WeatherData _weatherData;

    private const string UserNameKey = "UserName";
    private const string UserEmailKey = "UserEmail";
    private const string UserPhoneNumberKey = "UserPhoneNumber";

    public MyAccountPage(ApiService apiService, CitiesService citiesService, 
                         IRestService restService, IValidator validator, 
                         WeatherData weatherData)
    {
        InitializeComponent();
        _apiService = apiService;
        _citiesService = citiesService;
        _restService = restService;
        _validator = validator;
        _weatherData = weatherData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();       
    }

    private void LoadUserInfo()
    {
        lblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = lblUserName.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhoneNumber.Text = Preferences.Get(UserPhoneNumberKey, string.Empty);
    }

    private async void BtnSave_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneNumberKey, EntPhoneNumber.Text);

        await DisplayAlert("Success", "Your information was successfully saved", "OK");
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accessToken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _citiesService, 
                                                                         _restService, _validator, _weatherData));
    }
}