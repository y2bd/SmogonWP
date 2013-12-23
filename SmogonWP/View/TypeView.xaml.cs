using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class TypeView : PhoneApplicationPage
  {
    private bool _isNewInstance;

    public TypeView()
    {
      InitializeComponent();
      _isNewInstance = true;
    }

    private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
    {
      HelpPanel.Visibility = Visibility.Visible;
    }

    private void TypeView_OnBackKeyPress(object sender, CancelEventArgs e)
    {
      if (HelpPanel.Visibility == Visibility.Visible)
      {
        HelpPanel.Visibility = Visibility.Collapsed;
        e.Cancel = true;
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (e.NavigationMode != NavigationMode.Back)
      {
        this.State["tombstoned"] = true;
        Messenger.Default.Send(new TombstoneMessage<TypeViewModel>());
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (_isNewInstance && this.State.ContainsKey("tombstoned"))
      {
        Messenger.Default.Send(new RestoreMessage<TypeViewModel>());

        this.State.Remove("tombstoned");
        _isNewInstance = false;
      }
    }

  }
}