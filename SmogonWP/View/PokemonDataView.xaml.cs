using System;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

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

      Messenger.Default.Register<VmToViewMessage<string, PokemonDataView>>(this, onVmMessage);
      
      Unloaded += (sender, args) => Messenger.Default.Unregister(this);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        // Messenger.Default.Send(new TombstoneMessage<PokemonDataViewModel>());
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
      else if (e.NavigationMode != NavigationMode.Back)
      {
        // only do this if we haven't restored but it's not a new back navigation
        // tombstone ahead of time why not
        // this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<PokemonDataViewModel>());
      }
      
      if (e.NavigationMode != NavigationMode.Back)
      {
        OnLoaded.Begin();
      }
    }

    private void onVmMessage(VmToViewMessage<string, PokemonDataView> msg)
    {
      if (msg.Content == "loadedAnim")
      {
        AnimateIn.Begin();
      }
    }
  }
}