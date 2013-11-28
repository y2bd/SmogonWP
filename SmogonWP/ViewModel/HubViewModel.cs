using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using Schmogon.Data;
using SmogonWP.Services;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Windows.Phone.Speech.VoiceCommands;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class HubViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;

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

    private bool _isAppBarOpen = true;
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

    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchSearchDataNotifier { get; private set; }

    #endregion async handlers

    public HubViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

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
      if (!IsInDesignMode && !IsInDesignModeStatic)
        await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///BaseVCD.xml"));
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
      TrayService.AddJob("fetchall", "Fetching search data...");

      var pokeTask = _dataService.FetchAllPokemonAsync();
      var moveTask = _dataService.FetchAllMovesAsync();
      var move2Task = _dataService.FetchAllMovesAsync();
      var abilTask = _dataService.FetchAllAbilitiesAsync();
      var itemTask = _dataService.FetchAllItemsAsync();

      try
      {
        await Task.WhenAll(pokeTask, moveTask, move2Task, abilTask, itemTask);

        _allSearchItems = new List<ISearchItem>()
          .Concat(await pokeTask)
          .Concat(await moveTask)
          .Concat(await abilTask)
          .Concat(await itemTask)
          .OrderBy(i => i.Name)
          .ToList();
      }
      catch (Exception e)
      {
        if (!NetUtilities.IsNetwork())
        {
          MessageBox.Show(
            "Downloading search data requires an internet connection. Please get one of those and try again later.",
            "No internet!", MessageBoxButton.OK);
        }
        else
        {
          MessageBox.Show(
            "I'm sorry, but we couldn't load the search data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
            "Oh no!", MessageBoxButton.OK);

          Debugger.Break();
        }
      }

      TrayService.RemoveJob("fetchall");

      // TODO : Finish universal search on hubview AND make other VMs use the DataLoaderService instead of the schmogon client
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


  }
}
