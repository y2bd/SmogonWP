using System.Windows;
using System.Windows.Input;

namespace SmogonWP.Utilities.KeyboardHelper
{

  public class KeyboardDependency : DependencyObject
  {
    private static KeyboardHelper _keyboardHelper;

    private KeyboardDependency() { }

    static KeyboardDependency() { }

    public static readonly DependencyProperty IsTabEnabledProperty = DependencyProperty.RegisterAttached(
      "IsTabbingEnabled",
      typeof(bool),
      typeof(KeyboardDependency), new PropertyMetadata(OnIsTabbingEnabledChanged)
      );

    static void OnIsTabbingEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
      var fe = target as FrameworkElement;
      if (fe == null)
      {
        _keyboardHelper = null;
      }
      else
      {
        _keyboardHelper = new KeyboardHelper(fe);
        fe.KeyDown += fe_KeyDown;
      }

    }

    static void fe_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        _keyboardHelper.HandleReturnKey();
      }
    }


    public static bool GetIsTabEnabled(DependencyObject source)
    {
      return (bool)source.GetValue(IsTabEnabledProperty);
    }

    public static void SetIsTabEnabled(DependencyObject source, bool value) { source.SetValue(IsTabEnabledProperty, value); }

  }
}
