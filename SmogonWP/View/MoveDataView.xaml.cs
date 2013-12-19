using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class MoveDataView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public MoveDataView()
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
        Messenger.Default.Send(new TombstoneMessage<MoveDataViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<MoveDataViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
    }
  }
}