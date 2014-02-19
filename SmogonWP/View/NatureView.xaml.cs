using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;
using System.Windows.Navigation;

namespace SmogonWP.View
{
  public partial class NatureView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public NatureView()
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
        Messenger.Default.Send(new TombstoneMessage<NatureViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<NatureViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
      else if (e.NavigationMode != NavigationMode.Back)
      {
        Messenger.Default.Send(new TombstoneMessage<NatureViewModel>());
      }
    }
  }
}