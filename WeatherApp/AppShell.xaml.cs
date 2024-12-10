namespace WeatherApp
{
    using WeatherApp.Models;
    using WeatherApp.Pages;
    using WeatherApp.Services;
    using WeatherApp.Validations;

    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly CitiesService _citiesService;
        private readonly IRestService _restService;
        private readonly IValidator _validator;

        private WeatherData _weatherData;

        public AppShell(ApiService apiService, CitiesService citiesService, 
                        IRestService restService, IValidator validator, 
                        WeatherData weatherData)
        {
            InitializeComponent();
            _apiService = apiService;
            _citiesService = citiesService;
            _restService = restService;
            _validator = validator;
            _weatherData = weatherData;

            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_restService, _citiesService);
            var favoritesPage = new FavoritesPage(_citiesService);
            var weatherPage = new WeatherPage(_weatherData, _citiesService);
            var accountPage = new MyAccountPage(_apiService, _citiesService, _restService, _validator, _weatherData);
            var creditsPage = new CreditsPage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                    new ShellContent { Title = "Favorites", Icon = "heart", Content = favoritesPage },
                    new ShellContent { Title = "Weather", Icon = "weather", Content = weatherPage },
                    new ShellContent { Title = "My Account", Icon = "profile", Content = accountPage },
                    new ShellContent { Title = "Credits", Icon = "credits", Content = creditsPage },
                }
            });
        }
    }
}
