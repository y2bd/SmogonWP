using System.Windows;

namespace SmogonWP.Model
{
  public class PercentageWidthDP : DependencyObject
  {
    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof (double), typeof (PercentageWidthDP), new PropertyMetadata(default(double)));

    public double Value
    {
      get { return (double) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public PercentageWidthDP(double initialValue)
    {
      Value = initialValue;
    }
  }
}
