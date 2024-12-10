namespace WeatherApp.Models
{
    using System.Text.Json.Serialization;

    public class WeatherData
    {
        public string Name { get; set; }

        public Coord Coord { get; set; }

        public Weather[] Weather { get; set; }

        public string Base { get; set; }

        public Main Main { get; set; }

        public long Visibility { get; set; }

        public Wind Wind { get; set; }

        public Clouds Clouds { get; set; }

        public long Dt { get; set; }

        public Sys Sys { get; set; }

        [JsonPropertyName("id")]
        public long ApiId { get; set; }

        public long Cod { get; set; }
    }
}
