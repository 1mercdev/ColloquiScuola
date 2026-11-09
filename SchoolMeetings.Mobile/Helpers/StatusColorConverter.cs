using System.Globalization;

namespace SchoolMeetings.Mobile.Helpers
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Confirmed" => Color.FromArgb("#4CAF50"),  
                    "Completed" => Color.FromArgb("#2196F3"),  
                    "Cancelled" => Color.FromArgb("#F44336"),  
                    _ => Color.FromArgb("#9E9E9E")  
                };
            }

            return Color.FromArgb("#9E9E9E");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}