namespace WeatherApp.Models
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public long ApiId { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string Country { get; set; }

        public string? State { get; set; }
    }
}
