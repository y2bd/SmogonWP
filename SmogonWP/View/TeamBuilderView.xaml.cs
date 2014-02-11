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

namespace SmogonWP.View
{
  public partial class TeamBuilderView : PhoneApplicationPage
  {
    public TeamBuilderView()
    {
      InitializeComponent();
    }
    
    private void AddVisible_Completed(object sender, EventArgs e)
    {
      TeamNameInput.Focus();
    }

    private void TeamNameInput_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        TeamTypePicker.Focus();
      }
    }

    private void AddCollapsed_Completed(object sender, EventArgs e)
    {
      TeamSelector.Focus();
    }
  }
}