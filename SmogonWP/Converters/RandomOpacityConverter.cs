using System;
using System.Globalization;
using System.Windows.Data;

namespace SmogonWP.Converters
{
  /// <remarks>
  /// Not much of a converter!
  /// </remarks>
  public class RandomOpacityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var div = 0.1 + (new Random(value.GetHashCode())).NextDouble()/1.5;

      return div;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
