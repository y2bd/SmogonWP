using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class StatsView : PhoneApplicationPage
  {
    public StatsView()
    {
      InitializeComponent();
    }

    private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        DescriptionPivot.Focus();

        ((StatsViewModel) DataContext).LostFocusCommand.Execute(null);
      }
    }
  }
}