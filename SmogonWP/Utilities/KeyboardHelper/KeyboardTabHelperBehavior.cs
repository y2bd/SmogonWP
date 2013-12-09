// Keyboard helper from pauliom.wordpress.com, @pauliom

//using Microsoft.Expression.Interactivity;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SmogonWP.Utilities.KeyboardHelper
{
  public class KeyboardTabHelperBehavior : Behavior<UIElement>
  {
    private KeyboardHelper _keyboardHelper;
    protected override void OnAttached()
    {
      base.OnAttached();
      _keyboardHelper = new KeyboardHelper(this.AssociatedObject);
      this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
    }

    void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        _keyboardHelper.HandleReturnKey();
      }
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;

    }
  }
}
