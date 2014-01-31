using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Navigation;
using Windows.System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.New &&
          NavigationContext.QueryString.ContainsKey("voiceCommandName"))
      {
        var voiceCommandName = NavigationContext.QueryString["voiceCommandName"];

        if (voiceCommandName.Contains("TheVeryBest"))
        {
          var id = "qyXTgqJtoGM";

          var path = String.Format("vnd.youtube:{0}?vndapp=youtube", id);
          var uri = new Uri(path, UriKind.RelativeOrAbsolute);

          Launcher.LaunchUriAsync(uri);
        }
      }
    }
  }
}