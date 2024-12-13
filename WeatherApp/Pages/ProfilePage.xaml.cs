namespace WeatherApp.Pages;

using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Validations;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly IRestService _restService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private WeatherData _weatherData;

    public ProfilePage(ApiService apiService, FavoritesService favoritesService, 
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
        BtnProfileImage.Source = await GetProfileImage();
    }

    private async Task<string?> GetProfileImage()
    {
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage != null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }

                    break;

                default:
                    await DisplayAlert("Error", errorMessage ?? "Unable to get the image", "OK");
                    return defaultImage;
            }
        }

        if (response?.ImageUrl != null)
            return response.ImagePath;

        return defaultImage;
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file == null)
                return null;

            using var stream = await file.OpenReadAsync();

            using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "Functionality not supported by the device.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permissions not granted to access the camera or photo gallery.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error selecting the image: {ex.Message}", "OK");
        }

        return null;
    }

    private async void BtnProfileImage_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();

            if (imageArray == null)
            { 
                await DisplayAlert("Error", "Unable to load the image", "OK");
                return;
            }

            BtnProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);

            if (response.Data)
                await DisplayAlert("Success", "Image successfully sent.", "OK");

            else
                await DisplayAlert("Error", response.ErrorMessage ?? "Unknown error", "Cancel");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MyAccountPage(_apiService));
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new LoginPage(_apiService, _favoritesService, 
                                                 _restService, _validator, _weatherData);
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _favoritesService,
                                                 _restService, _validator, _weatherData));
    }

}