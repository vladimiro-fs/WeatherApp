namespace WeatherApp
{
    using Microsoft.Extensions.Logging;
    using WeatherApp.Models;
    using WeatherApp.Pages;
    using WeatherApp.Services;
    using WeatherApp.Validations;

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<FavoritesService>();
            builder.Services.AddSingleton<IRestService, RestService>();
            builder.Services.AddSingleton<IValidator, Validator>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<WeatherPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<WeatherData>();
            builder.Services.AddTransient<FavoritesPage>();

            return builder.Build();
        }
    }
}
