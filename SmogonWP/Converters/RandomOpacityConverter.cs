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
      double ease = 1.5;

      if (parameter != null)
      {
        Double.TryParse(parameter as string, out ease);
      }

      var div = 0.1 + (new Random(value.GetHashCode())).NextDouble()/ease;

      return div;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
