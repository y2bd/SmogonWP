using Windows.ApplicationModel.Store;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using Nito.AsyncEx;
using SchmogonDB;
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
    public const string UpdateKey = "update_" + "1.2";

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
          Text = "donate",
          IconUri = new Uri("/Assets/AppBar/appbar.marketplace.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(onDonateButtonClicked)
        },
        new MenuButtonViewModel
        {
          Text = "generation",
          IconUri = new Uri("/Assets/AppBar/refresh.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(onGenerationButtonClicked)
        },
        new MenuButtonViewModel
        {
          Text = "quicksearch",
          IconUri = new Uri("/Assets/AppBar/feature.search.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(onSearchButtonClicked)
        },
      };

      var generationButton = new MenuButtonViewModel
      {
        Text = "rate app",
        IconUri = new Uri("/Assets/AppBar/appbar.marketplace.png", UriKind.RelativeOrAbsolute),
        Command = new RelayCommand(onRateButtonClicked)
      };

      var donateButton = new MenuButtonViewModel
      {
        Text = "rate app",
        IconUri = new Uri("/Assets/AppBar/appbar.marketplace.png", UriKind.RelativeOrAbsolute),
        Command = new RelayCommand(onRateButtonClicked)
      };

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
            catch (Exception e)
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
      //DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.AddJob("fetchall", "Fetching search data..."));

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

      //DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("fetchall"));
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

    private void onGenerationButtonClicked()
    {
      var xyMode = _settingsService.Load("xymode", false);

      if (xyMode)
      {
        if (MessageBox.Show(
          "You're currently viewing XY data. Do you wish to go back to BW data? This requires restarting the app.",
          "Change Generation", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;

        _settingsService.Save("xymode", false);
        Application.Current.Terminate();
      }
      else
      {
        if (MessageBox.Show(
          "You're currently viewing BW data. Do you wish to go forwards to XY data? This requires restarting the app.",
          "Change Generation", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;

        _settingsService.Save("xymode", true);
        Application.Current.Terminate();
      }
    }

    private async void onDonateButtonClicked()
    {
      if (MessageBox.Show(
        "SmogonWP is free and ad-free, and it always will be, so donations from awesome people like you help support me as a developer.\n\n" +
        "Due to Windows Phone restrictions, I cannot let you donate a custom amount, so instead you can just donate as many times as you want.\n\n" +
        "Whether you donate once or a hundred times, I'll be eternally grateful.\n\n" +
        "Press OK to continue, or cancel if you wish to donate at a later time.", "Thanks for donating!",
        MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;

      ProductListing listing;

      try
      {
        var listingInfo = await CurrentApp.LoadListingInformationAsync();
        listing = listingInfo.ProductListings["small.donate"];
      }
      catch (Exception e)
      {
        Debugger.Break();
        MessageBox.Show("We couldn't load the donations page right now, please try again later.", "Oops!",
          MessageBoxButton.OK);

        return;
      }

      try
      {
        await CurrentApp.RequestProductPurchaseAsync(listing.ProductId, false);
      }
      catch (Exception)
      {
        // this means the user canceled the purchase
        return;
      }
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
        "I have fantastic news and not so great news!\n\n" +
        "The fantastic news is that Smogon *finally* published some XY data, which means that I've finally put XY data in the app, hurrah!\n\n" +
        "The not so great news is that Smogon hasn't put *all* of the XY data up yet. That means missing Pokemon, as well as tons of missing descriptions " +
        "and other stuff that you're used to. No more overviews at the moment :( \n\n" +
        "To please people who want the incomplete data *now*, there's a new button on the appbar below that looks like a refresh icon. This will let you switch between the old " +
        "BW data and the new XY data. Note that at the moment this requires an app restart. In the future, this'll be unnecessary (and you'll be able to switch between more things " +
        "than just BW and XY, kekeke). Note that as this data is in-progress, app support is in-progress, so some things won't work perfectly with XY data (such as voice commands). Please " +
        "bear with me in these crazy times.\n\n" +
        "There's also another new button on the appbar (or if you haven't rated that app yet (D:) it replaces that button). I've gotten requests for adding a donation option, so that's " +
        "what you can do now. SmogonWP is my pet project, and I'll never ever charge for it or muck it up with ads, but donations from people like you help me going. Don't feel obligated " +
        "though, I'm not going to hide app features behind a paywall or anything like that. Only donate if you actually want to.\n\n" +
        "I'm going to be on vacation for the next two weeks, so I won't be able to update the app for a while. Hopefully nothing bad happens, but feel free to email me if something does.",
        "Super Update News Wow!",
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
