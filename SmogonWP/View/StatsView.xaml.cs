using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;
using System.Windows.Input;

namespace SmogonWP.View
{
  public partial class StatsView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public StatsView()
    {
      InitializeComponent();

      _isNewInstance = true;
    }

    private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        DescriptionPivot.Focus();

        ((StatsViewModel) DataContext).LostFocusCommand.Execute(null);
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<StatsViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<StatsViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
      else if (e.NavigationMode != NavigationMode.Back)
      {
        Messenger.Default.Send(new TombstoneMessage<StatsViewModel>());
      }
    }
  }
}