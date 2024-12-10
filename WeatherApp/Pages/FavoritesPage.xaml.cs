namespace WeatherApp.Pages;

using WeatherApp.Services;

public partial class FavoritesPage : ContentPage
{
    private readonly CitiesService _citiesService;

	public FavoritesPage(CitiesService citiesService)
	{
		InitializeComponent();
        _citiesService = citiesService;
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
            var favoriteCities = await _citiesService.ReadAllAsync();

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
}