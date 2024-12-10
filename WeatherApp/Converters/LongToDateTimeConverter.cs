namespace WeatherApp.Converters
{
    using System.Globalization;

    public class LongToDateTimeConverter : IValueConverter
    {
        DateTime _time = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CultureInfo ptPtCulture = new CultureInfo("pt-PT");

            long dateTime = (long)value;

            return $"{_time.AddSeconds(dateTime).ToString(ptPtCulture)} UTC";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
