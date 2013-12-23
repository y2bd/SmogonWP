using System.IO;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Windows.Phone.Speech.VoiceCommands;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class HubViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;
    private readonly LiveTileService _tileService;
    private readonly ISettingsService _settingsService;

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

    public INotifyTaskCompletion FetchSearchDataNotifier { get; private set; }

    #endregion async handlers

    public HubViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService, LiveTileService tileService, ISettingsService settingsService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;
      _tileService = tileService;
      _settingsService = settingsService;

      _pokeSearchSender = new MessageSender<ItemSearchedMessage<Pokemon>>();
      _abilSearchSender = new MessageSender<ItemSearchedMessage<Ability>>();
      _moveSearchSender = new MessageSender<ItemSearchedMessage<Move>>();
      _itemSearchSender = new MessageSender<ItemSearchedMessage<Item>>();

      setupNavigation();
      setupAppBar();
      initializeVCD();
      scheduleSearchDataFetch();

      if (!IsInDesignMode)
        showWelcomePopup();
    }

    private void showWelcomePopup()
    {
      if (_settingsService.Load("haswelcomed", false)) return;

      const string message = @"Thanks for downloading SmogonWP! Before you use the app, let me tell you two important things.

First of all, this app is not and will never be a standard Pokedex app. This app will not tell you what routes you can catch Ralts on or at what level she'll evolve into a Gardevoir. That is not its purpose.

Second, as Pokemon X and Y have come out very recently, websites are still in the process of compiling information. Because of this, this app only contains data up to Black and White. You will not find X and Y Pokemon or moves in this app.

If you have any questions, sliding up the appbar at the bottom will give you the option to email me, the developer. Happy battling!";

      MessageBox.Show(message, "Hey there!", MessageBoxButton.OK);

      _settingsService.Save("haswelcomed", true);
    }

    private void showTipOfTheDay()
    {
      string[] tips = {
        "This app has an awesome forum (via Reddit) that you can visit by swiping up on the appbar below!",
        "This app has a live tile! Pin it to your start page to see!",
        "Although this app doesn't have X and Y data, you can still see how the Fairy type fares on the Type page!",
        "After choosing any of a Pokemon's movesets, you can click the stats button to jump straight to the stats calculator with values filled in!",
        "Many things are tappable! Try tapping on something and see what happens!",
        "Studies have shown that rating applications makes you at least twenty percent more awesome!",
        "On the Pokemon search page, you can filter your search by both type and tier at the same time!",
        "Many pages let you open up Bulbapedia! Just look for the minimized appbar at the bottom.",
        "Web scraping is hard! If you find an entry that looks wrong, email the developer and help him fix it!",
        "Blissey has an HP base stat of 255, the highest possible base stat!",
        "Shuckle takes the record for highest defense and special defense base stats, both being 230!",
        "Pokemon with multiple forms are all listed seperately for ease of searching!",
        "This app has a beta! Email the dev for information on how to join!",
        "The app is open-source! Swipe up on the appbar and open the 'about + credits' page if you want to learn more.",
        "You can swipe these annoying toast prompts away!",
        "The developer's favorite Pokemon is Haunter!",
        "On the Move page, tapping on the move's type will bring you to the Type page with fields filled in!",
        "Some colors in the app depend on your accent color, while others depends on Type colors!",
        "The app also looks fantastic with your phone's Light theme!",
        "If you have any suggestions for the app, you should email the developer!",
        "This app has voice commands! Try holding down your home button and saying 'Smogon, search for Gardevoir'!",
        "You can pin any of the menu items below to your start screen. Just press and hold!",
        "There's a master ball hidden underneath this message! No, seriously!"
      };

      var rnd = new Random();

      if (rnd.Next(5) > 1) return;

      var tip = tips[rnd.Next(tips.Length)];

      var toast = new ToastPrompt
      {
        Title = "Did you know?",
        Message = tip,
        TextWrapping = TextWrapping.Wrap,
      };

      toast.Show();
    }

    private void setupNavigation()
    {
      StratNavItems = new ObservableCollection<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Pokemon",
          Description = "Search through Pokemon and compose your team",
          NavigationPath = ViewModelLocator.PokemonSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Water]),
          IconPath = "/Assets/Icons/pokeball2.png"
        },
        new NavigationItemViewModel
        {
          Title = "Moves",
          Description = "Learn about every single move that your Pokemon can battle with",
          NavigationPath = ViewModelLocator.MoveSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Fire]),
          IconPath = "/Assets/Icons/conflict.png"
        },
        new NavigationItemViewModel
        {
          Title = "Abilities",
          Description = "Explore the various innate powers that your Pokemon possess",
          NavigationPath = ViewModelLocator.AbilitySearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Grass]),
          IconPath = "/Assets/Icons/idea.png"
        },
        new NavigationItemViewModel
        {
          Title = "Items",
          Description = "Shop through various items that can give boosts in battle",
          NavigationPath = ViewModelLocator.ItemSearchPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Ground]),
          IconPath = "/Assets/Icons/pill.png"
        },
      };

      ToolNavItems = new ObservableCollection<NavigationItemViewModel>
      {
        new NavigationItemViewModel
        {
          Title = "Natures",
          Description = "Check out how natures affect your Pokemon's stats",
          NavigationPath = ViewModelLocator.NaturePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Psychic]),
          IconPath = "/Assets/Icons/smile.png"
        },
        new NavigationItemViewModel
        {
          Title = "Types",
          Description = "See how typing affects your Pokemon's performance in battle",
          NavigationPath = ViewModelLocator.TypePath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Bug]),
          IconPath = "/Assets/Icons/fire.png"
        },
        new NavigationItemViewModel
        {
          Title = "Stat Calculator",
          Description = "Fine-tune the stats of your perfect Pokemon",
          NavigationPath = ViewModelLocator.StatsPath,
          BackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[Type.Dragon]),
          IconPath = "/Assets/Icons/calc.png"
        }
      };
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

      if (!_settingsService.Load("hasrated", false)) MenuButtons.Add(rateButton);

      MenuItems = new ObservableCollection<MenuItemViewModel>
      {
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
        }
      };
    }

    private async void initializeVCD()
    {
      /*
      if (!IsInDesignMode && !IsInDesignModeStatic)
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///BaseVCD.xml"));
       * */

      if (!IsInDesignMode && !IsInDesignModeStatic)
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///GenericVCD.xml"));
    }

    private void scheduleSearchDataFetch()
    {
      FetchSearchDataNotifier = NotifyTaskCompletion.Create(fetchSearchData());

      FetchSearchDataNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchSearchDataNotifier == null) return;

        if (FetchSearchDataNotifier.IsFaulted)
        {
          throw FetchSearchDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchSearchData()
    {
      var st = new Stopwatch();

      st.Start();
      await Task.Run(new Func<Task>(pleaseOffThread));
      st.Stop();
      Debug.WriteLine("JSON load: {0}", st.ElapsedMilliseconds);

      await updateLiveTile();
    }

    // better name in future
    // lol have fun jason
    private async Task pleaseOffThread()
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

      DispatcherHelper.CheckBeginInvokeOnUI(() =>
      {
        TrayService.RemoveJob("fetchall");
        showTipOfTheDay();
      });
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

    private void onSearchButtonClicked()
    {
      IsSearchPanelOpen = true;
      IsAppBarOpen = false;
    }

    private void onRateButtonClicked()
    {
      _settingsService.Save("hasrated", true);

      var mrt = new MarketplaceReviewTask();

      mrt.Show();
    }

    private void onEmailDevClicked()
    {
      var ect = new EmailComposeTask
      {
        To = "jason@y2bd.me",
        Subject = "SmogonWP Inquiry",
        Body = "(please include your phone model with your email)"
      };

      ect.Show();
    }

    private void onVisitForumClicked()
    {
      var wbt = new WebBrowserTask
      {
        Uri = new Uri("https://smogonwp.reddit.com/", UriKind.Absolute)
      };

      wbt.Show();
    }

    private void onCreditsClicked()
    {
      _navigationService.Navigate(ViewModelLocator.CreditsPath);
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
    }

    private void onPinToStart(NavigationItemViewModel nivm)
    {
      if (IsInDesignMode) return;

      var uri = new Uri(nivm.NavigationPath, UriKind.RelativeOrAbsolute);

      // don't create a tile if we already have one
      if (findExistingTile(uri) != null)
      {
        var already = new ToastPrompt
        {
          Title = "Sorry!",
          Message = "You already pinned this to your start screen!"
        };

        already.Show();

        return;
      }

      var name = TextUtils.ToTitleCase(nivm.Title);

      var iconName = Path.GetFileName(nivm.IconPath) ?? "pokeball2.png";

      var iconUri = new Uri(Path.Combine("/Assets/Tiles/Secondary/", iconName), UriKind.RelativeOrAbsolute);

      var tileData = createSecondaryTileData(name, iconUri);

      ShellTile.Create(uri, tileData);
    }

    private ShellTile findExistingTile(Uri uri)
    {
      return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.Equals(uri));
    }

    private StandardTileData createSecondaryTileData(string title, Uri iconUri)
    {
      var tileData = new StandardTileData
      {
        Title = title ?? string.Empty,
        BackgroundImage = iconUri ?? new Uri("", UriKind.Relative),
        BackTitle = "SmogonWP",
        BackBackgroundImage = iconUri ?? new Uri("", UriKind.Relative)
      };

      return tileData;
    }

    private async Task testDB()
    {
      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.AddJob("dbstuff", "DOING DB STUFF"));

      var db = new SchmogonDBClient();

      await db.InitializeDatabase();

      var poketask = db.FetchPokemonSearchDataAsync();
      var movetask = db.FetchMoveSearchDataAsync();
      var abiltask = db.FetchAbilitySearchDataAsync();
      var itemtask = db.FetchItemSearchDataAsync();

      await Task.WhenAll(poketask, movetask, abiltask, itemtask);

      var poke = await poketask;
      var move = await movetask;
      var abil = await abiltask;
      var item = await itemtask;

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("dbstuff"));

      var sel = poke.First(p => p.Name == "Gardevoir");

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.AddJob("dbstuff", "DOING POKEMON STUFF"));

      var pokenew = await db.FetchPokemonDataAsync(sel);

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("dbstuff"));
    }

    private async Task updateLiveTile()
    {
      var secretEnabled = _settingsService.Load<bool>("secret");

      try
      {
        await _tileService.GenerateFlipTileAsync(secretEnabled);
      }
      catch (Exception)
      {
        Debug.WriteLine("tile creation failed, aw well");
      }
    }

    private void onLogoTap()
    {
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
    }
  }
}
