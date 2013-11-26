using System.Windows.Controls;
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

    private void Panorama_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var pano = sender as Panorama;

      if (pano == null) return;

      //AppBar.Mode = pano.SelectedIndex == 0 ? ApplicationBarMode.Default : ApplicationBarMode.Minimized;
    }
  }
}