using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class ItemSearchView : PhoneApplicationPage
  {
    public ItemSearchView()
    {
      InitializeComponent();
    }

    private void Searchbox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) ItemList.Focus();
    }
  }
}