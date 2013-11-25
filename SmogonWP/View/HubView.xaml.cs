using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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