using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Abilities;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class AbilityDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<AbilitySearchMessage> _abilitySearchReceiver;
    
    // if a network request fails, we'll try again one more time
    // otherwise we'll give up and tell the user
    private bool _failedOnce;

    private string _pageLocation;

    #region props

    private string _name;
    public string Name
    {
      get
      {
        return _name.ToUpper();
      }
      set
      {
        if (_name != value)
        {
          _name = value;
          RaisePropertyChanged(() => Name);
        }
      }
    }			

    private AbilityDataItemViewModel _advm;
    public AbilityDataItemViewModel ADVM
    {
      get
      {
        return _advm;
      }
      set
      {
        if (_advm != value)
        {
          _advm = value;
          RaisePropertyChanged(() => ADVM);
        }
      }
    }
    
    private TrayService _trayService;
    public TrayService TrayService
    {
      get
      {
        return _trayService;
      }
      set
      {
        if (_trayService != value)
        {
          _trayService = value;
          RaisePropertyChanged(() => TrayService);
        }
      }
    }

    private ObservableCollection<MenuItemViewModel> _menuItems;
    public ObservableCollection<MenuItemViewModel> MenuItems
    {
      get
      {
        return _menuItems;
      }
      set
      {
        if (_menuItems != value)
        {
          _menuItems = value;
          RaisePropertyChanged(() => MenuItems);
        }
      }
    }			

    #endregion props

    #region commands

    private RelayCommand<CancelEventArgs> _backKeyPressCommand;
    public RelayCommand<CancelEventArgs> BackKeyPressCommand
    {
      get
      {
        return _backKeyPressCommand ??
               (_backKeyPressCommand = new RelayCommand<CancelEventArgs>(onBackKeyPressed));
      }
    }
    
    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchAbilityDataNotifier { get; private set; }

    #endregion async handlers

    public AbilityDataViewModel(ISchmogonClient schmogonClient, TrayService trayService)
    {
      _schmogonClient = schmogonClient;
      _trayService = trayService;

      _abilitySearchReceiver = new MessageReceiver<AbilitySearchMessage>(onAbilitySearched, true);
      
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchAbilityDataNotifier = NotifyTaskCompletion.Create(fetchAbilityData(null));
      }

      setupAppBar();
    }

    #region event handlers

    private void onAbilitySearched(AbilitySearchMessage msg)
    {
      // clear the current ability if it exists
      // otherwise we run into stack issues

      ADVM = null;

      Name = msg.Ability.Name;

      scheduleAbilityFetch(msg.Ability);
    }
    
    private void onBackKeyPressed(CancelEventArgs args)
    {
      return;
    }

    #endregion event handlers

    #region appbar

    private void setupAppBar()
    {
      var smogon = new MenuItemViewModel
      {
        Command = new RelayCommand(onOpenSmogonPressed),
        Text = "open Smogon in browser..."
      };

      var bulb = new MenuItemViewModel
      {
        Command = new RelayCommand(onOpenBulbapediaPressed),
        Text = "open Bulbapedia in browser..."
      };

      MenuItems = new ObservableCollection<MenuItemViewModel> {smogon, bulb};
    }

    private void onOpenSmogonPressed()
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri(SmogonPrefix + _pageLocation)
      };

      wbt.Show();
    }

    private void onOpenBulbapediaPressed()
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri(BulbaPrefix + Uri.EscapeDataString(toTitleCase(Name)))
      };

      wbt.Show();
    }

    private static string toTitleCase(string word)
    {
      IEnumerable<string> split = word.Split(' ').ToList();
      split = split.Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower());

      return string.Join(" ", split);
    }

    #endregion appbar
    
    private void scheduleAbilityFetch(Ability ability)
    {
      FetchAbilityDataNotifier = NotifyTaskCompletion.Create(fetchAbilityData(ability));

      FetchAbilityDataNotifier.PropertyChanged += (sender, args) =>
      {
        // we broked
        if (FetchAbilityDataNotifier == null) return;

        if (FetchAbilityDataNotifier.IsFaulted)
        {
          throw FetchAbilityDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchAbilityData(Ability ability)
    {
      TrayService.AddJob("fetchdata", "Fetching ability data...");
      
      AbilityData abilityData;

      try
      {
        abilityData = await _schmogonClient.GetAbilityDataAsync(ability);
      }
      catch (HttpRequestException)
      {
        reloadAbilityData(ability);
        return;
      }

      ADVM = new AbilityDataItemViewModel(abilityData);
      Name = ADVM.Name;

      _pageLocation = ability.PageLocation;

      TrayService.RemoveJob("fetchdata");
    }

    private void reloadAbilityData(Ability ability)
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "I'm sorry, but we couldn't load the ability data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "Downloading ability data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
      }
      else {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        scheduleAbilityFetch(ability);
      }
    }

    private void cleanup()
    {
      ADVM = null;
      FetchAbilityDataNotifier = null;
      TrayService.RemoveAllJobs();
    }
  }
}
