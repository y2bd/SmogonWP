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
  }
}