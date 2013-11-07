using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace SmogonWP.View
{
  public partial class HomeView : PhoneApplicationPage
  {
    public HomeView()
    {
      InitializeComponent();
    }

    private void EmailButton_OnClick(object sender, EventArgs e)
    {
      var ect = new EmailComposeTask
      {
        To = "jason@y2bd.me",
        Subject = "SmogonWP Inquiry"
      };

      ect.Show();
    }

    private void RedditButton_OnClick(object sender, EventArgs e)
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri("https://i.reddit.com/r/smogonwp", UriKind.Absolute)
      };

      wbt.Show();
    }
  }
}