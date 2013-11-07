using System;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class MoveSearchView : PhoneApplicationPage
  {
    public MoveSearchView()
    {
      InitializeComponent();
    }

    private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) MoveList.Focus();
    }

    private void FilterButton_OnClick(object sender, EventArgs e)
    {
      FilterPicker.Open();
    }
  }
}