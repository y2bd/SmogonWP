using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class ItemDataView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public ItemDataView()
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
        // Messenger.Default.Send(new TombstoneMessage<ItemDataViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<ItemDataViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
      else if (e.NavigationMode != NavigationMode.Back)
      {
        Messenger.Default.Send(new TombstoneMessage<ItemDataViewModel>());
      }

    }
  }
}