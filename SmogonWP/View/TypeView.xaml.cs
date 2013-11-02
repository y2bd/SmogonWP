﻿using System;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;

namespace SmogonWP.View
{
  public partial class TypeView : PhoneApplicationPage
  {
    public TypeView()
    {
      InitializeComponent();

      Messenger.Default.Register<VmToViewMessage<string, TypeView>>(this, onVmMessage);

      Unloaded += OnUnloaded;
    }
    
    private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Messenger.Default.Unregister(this);
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

    private void onVmMessage(VmToViewMessage<string, TypeView> msg)
    {
      if (msg.Content.Equals("switchToOffense"))
      {
        PhasePivot.SelectedIndex = 0;
      }
    }
  }
}