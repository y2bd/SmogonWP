using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Windows.System;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class HubView : PhoneApplicationPage
  {
    public HubView()
    {
      InitializeComponent();
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

    private void Visible_OnCompleted(object sender, EventArgs e)
    {
      QuickSearchBox.Focus();
      QuickSearchBox.SelectAll();
    }
  }
}