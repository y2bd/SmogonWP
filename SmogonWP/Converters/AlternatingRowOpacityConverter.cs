using System;
using System.Windows.Data;

namespace SmogonWP.Converters
{
  public class AlternatingRowOpacityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var row = value as int? ?? 0;

      return row%2 == 0 ? 0.4 : 0.25;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
