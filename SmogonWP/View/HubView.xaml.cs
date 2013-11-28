using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class HubView : PhoneApplicationPage
  {
    public HubView()
    {
      InitializeComponent();

      ToVisibleTransition.Storyboard.Completed += (sender, args) =>
      {
        QuickSearchBox.Focus();
        QuickSearchBox.SelectAll();
      };
    }

    private void Searchbox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) QuickSearchList.Focus();
    }
  }
}