using System;
using System.Windows.Data;

namespace SmogonWP.Converters
{
  public class WidthPercentageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var val = (double) value;
      var pct = val/100.0;

      var param = int.Parse((string)parameter);

      return pct*param;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
