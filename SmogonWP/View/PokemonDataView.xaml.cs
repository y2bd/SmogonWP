using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class PokemonDataView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public PokemonDataView()
    {
      InitializeComponent();

      // this will only be set to true on creation of this page
      // revisiting this page while it is still in memory will not set it to true
      _isNewInstance = true;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<PokemonDataViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<PokemonDataViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
    }
  }
}