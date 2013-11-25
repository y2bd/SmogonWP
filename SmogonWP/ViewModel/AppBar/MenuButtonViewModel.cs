using System;

namespace SmogonWP.ViewModel.AppBar
{
  public class MenuButtonViewModel : MenuItemViewModel
  {
    private Uri _iconUri;
    public Uri IconUri
    {
      get { return _iconUri; }
      set
      {
        _iconUri = value;
        RaisePropertyChanged(() => IconUri);
      }
    }
  }
}
