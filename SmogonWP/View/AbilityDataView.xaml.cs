using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class AbilityDataView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public AbilityDataView()
    {
      InitializeComponent();

      _isNewInstance = true;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<AbilityDataViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<AbilityDataViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
    }
  }
}