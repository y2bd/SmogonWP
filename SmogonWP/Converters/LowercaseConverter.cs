using System;
using System.Globalization;
using System.Windows.Data;

namespace SmogonWP.Converters
{
  public class LowercaseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value == null ? null : ((string)value).ToLowerInvariant();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
