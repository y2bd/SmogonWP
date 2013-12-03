using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Nito.AsyncEx;
using Schmogon.Data;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Pokemon;
using SchmogonDB;
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
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class HubViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;

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

    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchSearchDataNotifier { get; private set; }

    #endregion async handlers

    public HubViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

      _pokeSearchSender = new MessageSender<ItemSearchedMessage<Pokemon>>();
      _abilSearchSender = new MessageSender<ItemSearchedMessage<Ability>>();
      _moveSearchSender = new MessageSender<ItemSearchedMessage<Move>>();
      _itemSearchSender = new MessageSender<ItemSearchedMessage<Item>>();

      setupNavigation();
      setupAppBar();
      initializeVCD();
      scheduleSearchDataFetch();
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

      st.Reset();
      st.Start();
      await Task.Run(new Func<Task>(testDB));
      st.Stop();
      Debug.WriteLine("DB load: {0}", st.ElapsedMilliseconds);
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

        _allSearchItems = new List<ISearchItem>()
        .Concat((await pokeTask).Select(p => new PokemonItemViewModel(p)))
        .Concat((await moveTask).Select(m => new MoveItemViewModel(m)))
        .Concat((await abilTask).Select(a => new AbilityItemViewModel(a)))
        .Concat((await itemTask).Select(i => new ItemItemViewModel(i)))
        .OrderBy(i => i.Name)
        .ToList();

        DispatcherHelper.CheckBeginInvokeOnUI(() =>
        {
          FilteredSearchItems = new ObservableCollection<ISearchItem>();
          IsAppBarOpen = true;
        });
      }
      catch (Exception)
      {
        if (!NetUtilities.IsNetwork())
        {
          DispatcherHelper.CheckBeginInvokeOnUI(() => MessageBox.Show(
            "Downloading search data requires an internet connection. Please get one of those and try again later.",
            "No internet!", MessageBoxButton.OK));
        }
        else
        {
          DispatcherHelper.CheckBeginInvokeOnUI(() => MessageBox.Show(
            "I'm sorry, but we couldn't load the search data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
            "Oh no!", MessageBoxButton.OK));

          Debugger.Break();
        }
      }

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("fetchall"));
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

      var x = poke;

      DispatcherHelper.CheckBeginInvokeOnUI(() => TrayService.RemoveJob("dbstuff"));

      var sel = move.First(m => m.Name == "Shadow Ball");

      var res = await db.FetchMoveDataAsync(sel);
    }
  }
}
