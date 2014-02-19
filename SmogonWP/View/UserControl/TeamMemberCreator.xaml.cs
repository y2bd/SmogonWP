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

namespace SmogonWP.View.UserControl
{
  public partial class TeamMemberCreator : System.Windows.Controls.UserControl
  {
    public TeamMemberCreator()
    {
      InitializeComponent();
    }

    private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) DescriptionPivot.Focus();
    }
  }
}
