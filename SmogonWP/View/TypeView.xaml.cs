using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class TypeView : PhoneApplicationPage
  {
    public TypeView()
    {
      InitializeComponent();
    }

    private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
    {
      HelpPanel.Visibility = Visibility.Visible;
    }

    private void TypeView_OnBackKeyPress(object sender, CancelEventArgs e)
    {
      if (HelpPanel.Visibility == Visibility.Visible)
      {
        HelpPanel.Visibility = Visibility.Collapsed;
        e.Cancel = true;
      }
    }
  }
}