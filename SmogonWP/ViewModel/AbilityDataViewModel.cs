using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using Nito.AsyncEx;
using SchmogonDB.Model.Abilities;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class AbilityDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly IDataLoadingService _dataService;
    private readonly TombstoneService _tombstoneService;

    private readonly MessageReceiver<ItemSearchedMessage<Ability>> _abilitySearchReceiver;
    private readonly MessageReceiver<ItemSelectedMessage<Ability>> _pokemonAbilitySelectedReceiver; 
    
    private string _pageLocation;

    private Ability _rawAbility;

    #region props

    private string _name = string.Empty;
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

    public AbilityDataViewModel(IDataLoadingService dataService, TrayService trayService, TombstoneService tombstoneService)
    {
      _dataService = dataService;
      _trayService = trayService;
      _tombstoneService = tombstoneService;

      _abilitySearchReceiver = new MessageReceiver<ItemSearchedMessage<Ability>>(onAbilitySearched, true);
      _pokemonAbilitySelectedReceiver = new MessageReceiver<ItemSelectedMessage<Ability>>(onPokemonAbilitySelected, true);
      
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchAbilityDataNotifier = NotifyTaskCompletion.Create(fetchAbilityData(null));
      }

      setupAppBar();

      MessengerInstance.Register(this, new Action<TombstoneMessage<AbilityDataViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<AbilityDataViewModel>>(m => restore()));
    }

    #region event handlers

    private void onAbilitySearched(ItemSearchedMessage<Ability> msg)
    {
      // clear the current ability if it exists
      // otherwise we run into stack issues

      ADVM = null;

      Name = msg.Item.Name;

      scheduleAbilityFetch(msg.Item);
    }

    private void onPokemonAbilitySelected(ItemSelectedMessage<Ability> msg)
    {
      ADVM = null;

      Name = msg.Item.Name;

      scheduleAbilityFetch(msg.Item);
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

      _rawAbility = ability;

      AbilityData abilityData;

      try
      {
        abilityData = await _dataService.FetchAbilityDataAsync(ability);
      }
      catch (Exception)
      {
        MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        Debugger.Break();

        cleanup();

        return;
      }

      ADVM = new AbilityDataItemViewModel(abilityData);
      Name = ADVM.Name;

      _pageLocation = ability.PageLocation;

      TrayService.RemoveJob("fetchdata");

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = ADVM.AbilityData.Name,
        Category = "Ability Search",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private void cleanup()
    {
      ADVM = null;
      FetchAbilityDataNotifier = null;
      TrayService.RemoveAllJobs();
    }

    private async void tombstone()
    {
      if (_rawAbility != null)
        await _tombstoneService.Store("ts_ability", _rawAbility);

      //await _tombstoneService.Save();
    }

    private async void restore()
    {
      if (ADVM != null) return;

      var loaded = await _tombstoneService.Load<Ability>("ts_ability");

      if (loaded != null)
      {
        Name = loaded.Name;
        scheduleAbilityFetch(loaded);
      }
    }
  }
}
