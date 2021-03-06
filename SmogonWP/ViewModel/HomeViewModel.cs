﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using Windows.Phone.Speech.VoiceCommands;
using GalaSoft.MvvmLight;
using SmogonWP.Services;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class HomeViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;

    #region props
    private NavigationItemViewModel _selectedNavItem;
    public NavigationItemViewModel SelectedNavItem
    {
      get
      {
        return _selectedNavItem;
      }
      set
      {
        if (_selectedNavItem != value)
        {
          onNavItemSelected(value);

          // resets the selected item
          _selectedNavItem = null;
          RaisePropertyChanged(() => SelectedNavItem);
        }
      }
    }			

    private ObservableCollection<NavigationItemViewModel> _nivms;
    public ObservableCollection<NavigationItemViewModel> NIVMs
    {
      get
      {
        return _nivms;
      }
      set
      {
        if (_nivms != value)
        {
          _nivms = value;
          RaisePropertyChanged(() => NIVMs);
        }
      }
    }

    #endregion props

    public HomeViewModel(SimpleNavigationService navigationService)
    {
      _navigationService = navigationService;

      setup();
    }

    private async void setup()
    {
      NIVMs = new ObservableCollection<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Pokemon",
          Description = "Search for Pokemon and how to use them",
          NavigationPath = ViewModelLocator.PokemonSearchPath
        },
        new NavigationItemViewModel
        {
          Title = "Moves",
          Description = "Search for moves and their relevance",
          NavigationPath = ViewModelLocator.MoveSearchPath
        },
        new NavigationItemViewModel
        {
          Title = "Abilities",
          Description = "Search for abilities and how they change the field",
          NavigationPath = ViewModelLocator.AbilitySearchPath
        },
        new NavigationItemViewModel
        {
          Title = "Items",
          Description = "Search for items and which ones to use",
          NavigationPath = ViewModelLocator.ItemSearchPath
        },
        new NavigationItemViewModel
        {
          Title = "Natures",
          Description = "See how natures affect your Pokemon",
          NavigationPath = ViewModelLocator.NaturePath
        },
        new NavigationItemViewModel
        {
          Title = "Types",
          Description = "Compare types and how they interact with each other",
          NavigationPath = ViewModelLocator.TypePath
        }
      };

      if (!IsInDesignMode && !IsInDesignModeStatic)
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///BaseVCD.xml"));
    }

    private void onNavItemSelected(NavigationItemViewModel item)
    {
      if (item == null) return;

      if (string.IsNullOrEmpty(item.NavigationPath))
      {
        MessageBox.Show("That feature isn't available yet! Stay tuned though, it should be coming soon.");
        return;
      }

      _navigationService.Navigate(item.NavigationPath);
    }
  }
}
