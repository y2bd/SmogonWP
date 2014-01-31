using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using SmogonWP.Messages;
using SmogonWP.ViewModel;

namespace SmogonWP.View
{
  public partial class PokemonSearchView : PhoneApplicationPage
  {
    private readonly ApplicationBarSubmenu _filterSubmenu;

    private readonly ApplicationBarSubmenuItem _clear;
    private readonly ApplicationBarSubmenuItem _bySecondType;

    public PokemonSearchView()
    {
      InitializeComponent();

      var byType = new ApplicationBarSubmenuItem { Header = "by type..." };
      var byTier = new ApplicationBarSubmenuItem { Header = "by tier..." };

      _bySecondType = new ApplicationBarSubmenuItem { Header = "by secondary type...", IsEnabled = false };
      _clear = new ApplicationBarSubmenuItem { Header = "reset filters", IsEnabled = false };

      byType.Click += (sender, args) => TypePicker.Open();
      byTier.Click += (sender, args) => TierPicker.Open();

      _bySecondType.Click += (sender, args) => SecondaryTypePicker.Open();

      _clear.Click += (sender, args) =>
      {
        TypePicker.SelectedIndex = 0;
        SecondaryTypePicker.SelectedIndex = 0;
        TierPicker.SelectedIndex = 0;
      };

      _filterSubmenu = new ApplicationBarSubmenu
      {
        Items = { byType, _bySecondType, byTier, _clear }
      };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.New)
      {
        if (NavigationContext.QueryString.ContainsKey("voiceCommandName"))
        {
          var voiceCommandName = NavigationContext.QueryString["voiceCommandName"];

          if (voiceCommandName.Contains("SearchPokemon"))
          {
            var pokemonName = NavigationContext.QueryString["Pokemon"];

            Messenger.Default.Send(new ViewToVmMessage<string, PokemonSearchViewModel>(pokemonName));
          }
        }
      }
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

      if (sender == TypePicker)
      {
        if (TypePicker.SelectedIndex > 0)
        {
          _bySecondType.IsEnabled = true;
        }
        else
        {
          _bySecondType.IsEnabled = false;
        }
      }
    }

    private void SearchBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
      SearchBox.SelectAll();
    }

  }
}