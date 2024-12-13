namespace WeatherApp.Services
{
    using SQLite;
    using WeatherApp.Models;

    public class FavoritesService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritesService()
        {
            var dbPath = Path.Combine(
                            Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                            "favorites.db");

            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<FavoriteCity>();
        }
       
        public async Task<FavoriteCity> ReadAsync(long apiId)
        {
            try
            {
                return await _database.Table<FavoriteCity>()
                        .Where(c => c.ApiId == apiId)
                        .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FavoriteCity>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<FavoriteCity>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateAsync(FavoriteCity favoriteCity)
        {
            try
            {
                await _database.InsertAsync(favoriteCity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(FavoriteCity favoriteCity)
        {
            try
            {
                await _database.DeleteAsync(favoriteCity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
