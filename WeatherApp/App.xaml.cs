namespace WeatherApp
{
    using WeatherApp.Models;
    using WeatherApp.Pages;
    using WeatherApp.Services;
    using WeatherApp.Validations;

    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly CitiesService _citiesService;
        private readonly IRestService _restService;
        private readonly IValidator _validator;

        private WeatherData _weatherData;

        public App(ApiService apiService, CitiesService citiesService, 
                   IRestService restService, IValidator validator, 
                   WeatherData weatherData)
        {
            InitializeComponent();
            _apiService = apiService;
            _citiesService = citiesService;
            _restService = restService;
            _validator = validator;
            _weatherData = weatherData;

            SetMainPage();     
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accessToken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new LoginPage(_apiService, _citiesService, 
                                                            _restService, _validator, _weatherData));
                return;
            }

            MainPage = new AppShell(_apiService, _citiesService, 
                                    _restService, _validator, _weatherData);
        }
    }
}
