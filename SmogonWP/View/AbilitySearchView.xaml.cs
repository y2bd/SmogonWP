using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class AbilitySearchView : PhoneApplicationPage
  {
    public AbilitySearchView()
    {
      InitializeComponent();
    }

    private void Searchbox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) AbilityList.Focus();
    }
  }
}