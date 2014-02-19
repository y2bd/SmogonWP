using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using Nito.AsyncEx;
using SchmogonDB.Model;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Phone.Speech.VoiceCommands;

namespace SmogonWP.ViewModel
{
  public class HubViewModel : ViewModelBase
  {
    public const string UpdateKey = "update_" + "1.1.5";

    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;
    private readonly LiveTileService _tileService;
    private readonly ISettingsService _settingsService;
    private readonly RateService _rateService;

    private readonly MessageSender<ItemSearchedMessage<Pokemon>> _pokeSearchSender;
    private readonly MessageSender<ItemSearchedMessage<Ability>> _abilSearchSender;
    private readonly MessageSender<ItemSearchedMessage<Move>> _moveSearchSender;
    private readonly MessageSender<ItemSearchedMessage<Item>> _itemSearchSender;

    private IEnumerable<ISearchItem> _allSearchItems;

    #region props

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

    private ObservableCollection<NavigationItemViewModel> _stratNavItems;
    public ObservableCollection<NavigationItemViewModel> StratNavItems
    {
      get
      {
        return _stratNavItems;
      }
      set
      {
        if (_stratNavItems != value)
        {
          _stratNavItems = value;
          RaisePropertyChanged(() => StratNavItems);
        }
      }
    }

    private NavigationItemViewModel _selectedStratItem;
    public NavigationItemViewModel SelectedStratItem
    {
      get
      {
        return _selectedStratItem;
      }
      set
      {
        if (_selectedStratItem != value)
        {
          onNavItemSelected(value);

          _selectedStratItem = null;
          RaisePropertyChanged(() => SelectedStratItem);
        }
      }
    }

    private ObservableCollection<NavigationItemViewModel> _toolNavItems;
    public ObservableCollection<NavigationItemViewModel> ToolNavItems
    {
      get
      {
        return _toolNavItems;
      }
      set
      {
        if (_toolNavItems != value)
        {
          _toolNavItems = value;
          RaisePropertyChanged(() => ToolNavItems);
        }
      }
    }

    private NavigationItemViewModel _selectedToolItem;
    public NavigationItemViewModel SelectedToolItem
    {
      get
      {
        return _selectedToolItem;
      }
      set
      {
        if (_selectedToolItem != value)
        {
          onNavItemSelected(value);

          _selectedToolItem = null;
          RaisePropertyChanged(() => SelectedToolItem);
        }
      }
    }

    private bool _isSearchPanelOpen;
    public bool IsSearchPanelOpen
    {
      get
      {
        return _isSearchPanelOpen;
      }
      set
      {
        if (_isSearchPanelOpen != value)
        {
          _isSearchPanelOpen = value;
          RaisePropertyChanged(() => IsSearchPanelOpen);
        }
      }
    }

    private bool _isAppBarOpen;
    public bool IsAppBarOpen
    {
      get
      {
        return _isAppBarOpen;
      }
      set
      {
        if (_isAppBarOpen != value)
        {
          _isAppBarOpen = value;
          RaisePropertyChanged(() => IsAppBarOpen);
        }
      }
    }

    private ObservableCollection<MenuButtonViewModel> _menuButtons;
    public ObservableCollection<MenuButtonViewModel> MenuButtons
    {
      get
      {
        return _menuButtons;
      }
      set
      {
        if (_menuButtons != value)
        {
          _menuButtons = value;
          RaisePropertyChanged(() => MenuButtons);
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

    private string _filter;
    public string Filter
    {
      get
      {
        return _filter;
      }
      set
      {
        if (_filter != value)
        {
          _filter = value;
          RaisePropertyChanged(() => Filter);
        }
      }
    }

    private ObservableCollection<ISearchItem> _filteredSearchItems;
    public ObservableCollection<ISearchItem> FilteredSearchItems
    {
      get
      {
        return _filteredSearchItems;
      }
      set
      {
        if (_filteredSearchItems != value)
        {
          _filteredSearchItems = value;
          RaisePropertyChanged(() => FilteredSearchItems);
        }
      }
    }

    public bool ItemsFound
    {
      get
      {
        return _filteredSearchItems != null && _filteredSearchItems.Count > 0;
      }
    }

    private ISearchItem _selectedSearchItem;
    public ISearchItem SelectedSearchItem
    {
      get
      {
        return _selectedSearchItem;
      }
      set
      {
        if (_selectedSearchItem != value)
        {
          onSearchItemSelected(value);

          _selectedSearchItem = null;
          RaisePropertyChanged(() => SelectedSearchItem);
        }
      }
    }

    #endregion props

    #region commands

    private RelayCommand<CancelEventArgs> _backKeyPressedCommand;
    public RelayCommand<CancelEventArgs> BackKeyPressedCommand
    {
      get
      {
        return _backKeyPressedCommand ??
               (_backKeyPressedCommand = new RelayCommand<CancelEventArgs>(onBackKeyPressed));
      }
    }

    private RelayCommand<KeyEventArgs> _queryChangedCommand;
    public RelayCommand<KeyEventArgs> QueryChangedCommand
    {
      get
      {
        return _queryChangedCommand ??
               (_queryChangedCommand = new RelayCommand<KeyEventArgs>(onQueryChanged));
      }
    }

    private RelayCommand<NavigationItemViewModel> _pinToStartCommand;
    public RelayCommand<NavigationItemViewModel> PinToStartCommand
    {
      get
      {
        return _pinToStartCommand ??
               (_pinToStartCommand = new RelayCommand<NavigationItemViewModel>(onPinToStart));
      }
    }

    private RelayCommand _logoTapCommand;
    public RelayCommand LogoTapCommand
    {
      get
      {
        return _logoTapCommand ??
               (_logoTapCommand = new RelayCommand(onLogoTap));
      }
    }

    #endregion commands

    #region async handlers

    public INotifyTaskCompletion StartupTasksNotifier { get; private set; }

    #endregion async handlers

    public HubViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService, LiveTileService tileService, ISettingsService settingsService, RateService rateService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;
      _tileService = tileService;
      _settingsService = settingsService;
      _rateService = rateService;

      _pokeSearchSender = new MessageSender<ItemSearchedMessage<Pokemon>>();
      _abilSearchSender = new MessageSender<ItemSearchedMessage<Ability>>();
      _moveSearchSender = new MessageSender<ItemSearchedMessage<Move>>();
      _itemSearchSender = new MessageSender<ItemSearchedMessage<Item>>();

      setupNavigation();
      setupAppBar();
      initializeVCD();
      scheduleStartupTasks();

      if (!IsInDesignMode)
      {
        showWelcomePopup();
      }
    }

    private void showWelcomePopup()
    {
      if (_settingsService.Load("haswelcomed", false)) return;

      const string message = @"Thanks for downloading SmogonWP! Before you use the app, let me tell you two important things.

First of all, this app is not and will never be a standard Pokedex app. This app will not tell you what routes you can catch Ralts on or at what level she'll evolve into a Gardevoir. That is not its purpose.

Second, as Pokemon X and Y have come out very recently, websites are still in the process of compiling information. I've currently updated all BW Pokemon to their XY forms, and am starting to trickle in XY Pokemon. Stay tuned!

If you have any questions, sliding up the appbar at the bottom will give you the option to email me, the developer. Happy battling!";

      MessageBox.Show(message, "Hey there!", MessageBoxButton.OK);

      _settingsService.Save("haswelcomed", true);
    }

    private void setupNavigation()
    {
      StratNavItems = new ObservableCollection<NavigationItemViewModel>(NavigationItemFactory.MakeStratNavItems());

      ToolNavItems = new ObservableCollection<NavigationItemViewModel>(NavigationItemFactory.MakeToolNavItems());
    }

    private void setupAppBar()
    {
      MenuButtons = new ObservableCollection<MenuButtonViewModel>
      {
        new MenuButtonViewModel
        {
          Text = "quicksearch",
          IconUri = new Uri("/Assets/AppBar/feature.search.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(onSearchButtonClicked)
        },
      };

      var rateButton = new MenuButtonViewModel
      {
        Text = "rate app",
        IconUri = new Uri("/Assets/AppBar/appbar.marketplace.png", UriKind.RelativeOrAbsolute),
        Command = new RelayCommand(onRateButtonClicked)
      };

      if (!_rateService.HasRated()) MenuButtons.Add(rateButton);

      MenuItems = new ObservableCollection<MenuItemViewModel>
      {
        new MenuItemViewModel
        {
          Text = "change live tile...",
          Command = new RelayCommand(onChangeTileClicked)
        },
        new MenuItemViewModel
        {
          Text = "email developer...",
          Command = new RelayCommand(onEmailDevClicked)
        },
        new MenuItemViewModel
        {
          Text = "visit forum...",
          Command = new RelayCommand(onVisitForumClicked)
        },
        new MenuItemViewModel
        {
          Text = "about + credits",
          Command = new RelayCommand(onCreditsClicked)
        },
#if DEBUG
        new MenuItemViewModel
        {
          Text = "recreate database",
          Command = new RelayCommand(async () =>
          {
            try
            {
              TrayService.AddJob("rec", "Recreating database...");

              // this is not good code, but it is not meant to be
              // this is for me only to use
              var sm = (SchmogonDBClient) ((DataLoadingService) _dataService).Client;
              await sm.RecreateDatabase();

              TrayService.RemoveJob("rec");
            }
            catch (Exception)
            {
              Debugger.Break();
            }
          })
        }
#endif
      };
    }

    private async void initializeVCD()
    {
      if (IsInDesignMode || IsInDesignModeStatic) return;

      try
      {
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///GenericVCD.xml"));
      }
      catch (Exception)
      {
        Debug.WriteLine("Welp, voice commands don't work.");
      }
    }

    private void scheduleStartupTasks()
    {
      StartupTasksNotifier = NotifyTaskCompletion.Create(performStartupTasks());

      StartupTasksNotifier.PropertyChanged += (sender, args) =>
      {
        if (StartupTasksNotifier == null) return;

        if (StartupTasksNotifier.IsFaulted)
        {
          throw StartupTasksNotifier.InnerException;
        }
      };
    }

    private async Task performStartupTasks()
    {
      await Task.Run(new Func<Task>(fetchSearchData));

      Debug.WriteLine("Finished tile");

      TrayService.RemoveAllJobs();

      // this is really not something to crash the phone over
      try
      {
        // this is for aesthetics more than anything
        await Task.Delay(50);

        if (!showUpdatePrompt())
        if (!_rateService.CheckForRateReminder())
        {
          TipService.ShowTipOfTheDay();
        }
      }
      catch (Exception)
      {
        Debug.WriteLine("either the rate or the tipoftheday service failed, eh");
      }

      Debug.WriteLine("Finished reminders");

      await updateLiveTile();

      Debug.WriteLine("Finished tile");
    }

    private async Task fetchSearchData()
    {
      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.AddJob("fetchall", "Fetching search data..."));

      var pokeTask = _dataService.FetchAllPokemonAsync();
      var moveTask = _dataService.FetchAllMovesAsync();
      var abilTask = _dataService.FetchAllAbilitiesAsync();
      var itemTask = _dataService.FetchAllItemsAsync();

      try
      {
        await Task.WhenAll(pokeTask, moveTask, abilTask, itemTask);

        var poke = await pokeTask;
        var move = await moveTask;
        var abil = await abilTask;
        var item = await itemTask;

        DispatcherHelper.CheckBeginInvokeOnUI(() =>
        {
          _allSearchItems = new List<ISearchItem>()
            .Concat((poke).Select(p => new PokemonItemViewModel(p)))
            .Concat((move).Select(m => new MoveItemViewModel(m)))
            .Concat((abil).Select(a => new AbilityItemViewModel(a)))
            .Concat((item).Select(i => new ItemItemViewModel(i)))
            .OrderBy(i => i.Name)
            .ToList();

          FilteredSearchItems = new ObservableCollection<ISearchItem>();
          IsAppBarOpen = true;
        });
      }
      catch (Exception)
      {
        DispatcherHelper.CheckBeginInvokeOnUI(() => MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK));

        Debugger.Break();
      }

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("fetchall"));
    }

    private void onNavItemSelected(NavigationItemViewModel item)
    {
      if (item == null || string.IsNullOrEmpty(item.NavigationPath)) return;

      _navigationService.Navigate(item.NavigationPath);
    }

    private void onSearchButtonClicked()
    {
      IsSearchPanelOpen = true;
      IsAppBarOpen = false;
    }

    private void onRateButtonClicked()
    {
      _rateService.CheckForRateReminder();

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = "Via Store Button",
        Category = "Rating",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private void onEmailDevClicked()
    {
      _rateService.PopupEmailTask("TYPHLOSION");
    }

    private void onVisitForumClicked()
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri("https://smogonwp.reddit.com/", UriKind.Absolute)
      };

      wbt.Show();

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = "Visit Reddit",
        Category = "Feature Usage",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private void onCreditsClicked()
    {
      _navigationService.Navigate(ViewModelLocator.CreditsPath);
    }

    private void onChangeTileClicked()
    {
      _navigationService.Navigate(ViewModelLocator.LiveTilePath);
    }

    private void onBackKeyPressed(CancelEventArgs e)
    {
      if (IsSearchPanelOpen)
      {
        e.Cancel = true;

        IsSearchPanelOpen = false;
        IsAppBarOpen = true;
      }
    }

    private void onQueryChanged(KeyEventArgs args)
    {
      if (_allSearchItems == null || Filter == null) return;
      if (args.Key != Key.Enter) return;

      if (string.IsNullOrWhiteSpace(Filter))
      {
        FilteredSearchItems = new ObservableCollection<ISearchItem>();
        return;
      }

      FilteredSearchItems = new ObservableCollection<ISearchItem>(
        _allSearchItems.Where(
          m => m.Name.ToLower().Contains(Filter.ToLower().Trim())
        ).OrderBy(m => m.Name)
      );

      RaisePropertyChanged(() => ItemsFound);
    }

    private void onSearchItemSelected(ISearchItem item)
    {
      if (item is PokemonItemViewModel)
      {
        _pokeSearchSender.SendMessage(new ItemSearchedMessage<Pokemon>((item as PokemonItemViewModel).Pokemon));
        _navigationService.Navigate(ViewModelLocator.PokemonDataPath);
      }
      else if (item is AbilityItemViewModel)
      {
        _abilSearchSender.SendMessage(new ItemSearchedMessage<Ability>((item as AbilityItemViewModel).Ability));
        _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
      }
      else if (item is MoveItemViewModel)
      {
        _moveSearchSender.SendMessage(new ItemSearchedMessage<Move>((item as MoveItemViewModel).Move));
        _navigationService.Navigate(ViewModelLocator.MoveDataPath);
      }
      else if (item is ItemItemViewModel)
      {
        _itemSearchSender.SendMessage(new ItemSearchedMessage<Item>((item as ItemItemViewModel).Item));
        _navigationService.Navigate(ViewModelLocator.ItemDataPath);
      }

      IsSearchPanelOpen = false;
      IsAppBarOpen = true;

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = "Quick Search",
        Category = "Feature Usage",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    private void onPinToStart(NavigationItemViewModel nivm)
    {
      if (IsInDesignMode) return;
      
      var name = TextUtils.ToTitleCase(nivm.Title);
      var navUri = new Uri(nivm.NavigationPath, UriKind.RelativeOrAbsolute);

      var iconName = Path.GetFileName(nivm.IconPath) ?? "pokeball2.png";
      var iconUri = new Uri(Path.Combine("/Assets/Tiles/Secondary/", iconName), UriKind.RelativeOrAbsolute);
      
      // don't create a tile if we already have one
      if (!_tileService.CreateSecondaryTile(name, navUri, iconUri))
      {
        var already = new ToastPrompt
        {
          Title = "Sorry!",
          Message = "You already pinned this to your start screen!"
        };

        already.Show();
      }

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = "Secondary Tiles",
        Category = "Feature Usage",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
        ObjectName = name
      });
    }

    private async Task updateLiveTile()
    {
      try
      {
        await _tileService.GenerateFlipTileAsync();
      }
      catch (Exception)
      {
        Debug.WriteLine("tile creation failed, aw well");
      }
    }

    private void onLogoTap()
    {
      /*
      var prompt = new CustomMessageBox
      {
        Caption = "You found the master ball!",
        Message = "Wouid you like to enable the super-secret wide tile for this app?\n\nYou might need to close the app fully and start it again to get the tile to appear.",
        LeftButtonContent = "enable",
        RightButtonContent = "disable",
      };

      prompt.Dismissed += async (sender, args) =>
      {
        if (args.Result == CustomMessageBoxResult.LeftButton)
        {
          _settingsService.Save("secret", true);
          await updateLiveTile();
        }
        else if (args.Result == CustomMessageBoxResult.RightButton)
        {
          _settingsService.Save("secret", false);
          await updateLiveTile();
        }
      };

      prompt.Show();
      */

      if (MessageBox.Show(
        "This used to be the main way of changing the live tile, but now you can get there via the appbar below. Do you want to change your live tile?",
        "You found the master ball!",
        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        _navigationService.Navigate(ViewModelLocator.LiveTilePath);
      };

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = "Logo Tap",
        Category = "Feature Usage",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });
    }

    // returns true if the update prompt was shown
    private bool showUpdatePrompt()
    {
      // if we already have shown it, don't do it again stupid
      if (_settingsService.SettingRegistered(UpdateKey)) return false;

      // if we're opening the app for the first time, don't do it either
      if (!_settingsService.SettingRegistered("haswelcomed")) return false;

      MessageBox.Show(
        "Hey everyone! Just another little update.\n\n" +
        "There are some more new live tiles! Remember to go check them out and reset your chosen tile if it changed (sorry about that). " +
        "There's also an option to disable the tile from flipping if you don't like that (this was requested by a couple of you, thanks for the suggestion!).\n\n" +
        "I also fixed some small but embarrassing bugs. If you noticed the app crashing a bit when closing or reopening it (especially if you have a low-memory device) " +
        "that shouldn't happen any more.\n\n" +
        "As always, if you need anything (like even more live tiles!) feel free to email me. Happy battling!",
        "Update Notes (Please Read)",
        MessageBoxButton.OK);

      _settingsService.Save(UpdateKey, true);

      WebAnalyticsService.Current.Log(new AnalyticsEvent
      {
        Name = RateService.GetAppVersion(),
        Category = "Update",
        HitType = HitType.Event,
        ObjectType = this.GetType().Name,
      });

      return true;
    }
  }
}
