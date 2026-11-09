using System.Globalization;

namespace SchoolMeetings.Mobile.Helpers
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isAvailable)
            {
                return isAvailable
                    ? Color.FromArgb("#4CAF50") // verde
                    : Color.FromArgb("#F44336"); // rosso
            }

            return Color.FromArgb("#9E9E9E"); // grigio
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}