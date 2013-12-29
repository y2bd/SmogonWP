using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
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
    
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back)
      {
        SearchBox.KeyUp -= Searchbox_OnKeyUp;
      }
    }
  }
}