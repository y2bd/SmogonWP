﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace SmogonWP.View
{
  public partial class PokemonSearchView : PhoneApplicationPage
  {
    private readonly ApplicationBarSubmenu _filterSubmenu;

    private readonly ApplicationBarSubmenuItem _clear;

    public PokemonSearchView()
    {
      InitializeComponent();

      var byType = new ApplicationBarSubmenuItem { Header = "by type..." };
      var byTier = new ApplicationBarSubmenuItem { Header = "by tier..." };
      _clear = new ApplicationBarSubmenuItem { Header = "reset filters", IsEnabled = false };

      byType.Click += (sender, args) => TypePicker.Open();
      byTier.Click += (sender, args) => TierPicker.Open();

      _clear.Click += (sender, args) =>
      {
        TypePicker.SelectedIndex = 0;
        TierPicker.SelectedIndex = 0;
      };

      _filterSubmenu = new ApplicationBarSubmenu
      {
        Items = { byType, byTier, _clear }
      };
    }

    private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) PokemonList.Focus();
    }

    private void FilterButton_OnClick(object sender, EventArgs e)
    {
      _filterSubmenu.Show();
    }

    private void FilterPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      _clear.IsEnabled = TypePicker.SelectedIndex > 0 || TierPicker.SelectedIndex > 0;
    }

    private void SearchBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
      SearchBox.SelectAll();
    }
  }
}