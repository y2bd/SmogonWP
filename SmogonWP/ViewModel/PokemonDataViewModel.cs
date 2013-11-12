using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Schmogon.Data.Pokemon;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class PokemonDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly ISchmogonClient _schmogonClient;
    private readonly SimpleNavigationService _navigationService;

    private readonly MessageReceiver<PokemonSearchMessage> _pokemonSearchReceiver;
    private readonly MessageSender<PokemonTypeSelectedMessage> _pokemonTypeSelectedSender;
    private readonly MessageSender<PokemonAbilitySelectedMessage> _pokemonAbilitySelectedSender;
    private readonly MessageSender<PokemonMoveSelectedMessage> _pokemonMoveSelectedSender;
    private readonly MessageSender<MovesetSelectedMessage> _movesetSelectedSender; 

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

    private PokemonDataItemViewModel _pdvm;
    public PokemonDataItemViewModel PDVM
    {
      get
      {
        return _pdvm;
      }
      set
      {
        if (_pdvm != value)
        {
          _pdvm = value;
          RaisePropertyChanged(() => PDVM);
        }
      }
    }

    private AbilityItemViewModel _selectedAbility;
    public AbilityItemViewModel SelectedAbility
    {
      get
      {
        return _selectedAbility;
      }
      set
      {
        if (_selectedAbility != value)
        {
          onAbilitySelected(value);

          _selectedAbility = null;
          RaisePropertyChanged(() => SelectedAbility);
        }
      }
    }

    private TypeItemViewModel _selectedtype;
    public TypeItemViewModel SelectedType
    {
      get
      {
        return _selectedtype;
      }
      set
      {
        if (_selectedtype != value)
        {
          onTypeSelected(value);

          _selectedtype = null;
          RaisePropertyChanged(() => SelectedType);
        }
      }
    }

    private MoveItemViewModel _selectedMove;
    public MoveItemViewModel SelectedMove
    {
      get
      {
        return _selectedMove;
      }
      set
      {
        if (_selectedMove != value)
        {
          onMoveSelected(value);

          _selectedMove = null;
          RaisePropertyChanged(() => SelectedMove);
        }
      }
    }

    private MovesetItemViewModel _selectedMoveset;
    public MovesetItemViewModel SelectedMoveset
    {
      get
      {
        return _selectedMoveset;
      }
      set
      {
        if (_selectedMoveset != value)
        {
          onMovesetSelected(value);

          _selectedMoveset = null;
          RaisePropertyChanged(() => SelectedMoveset);
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

    #region async handlers

    public INotifyTaskCompletion FetchPokemonDataNotifier { get; private set; }

    #endregion async handlers

    public PokemonDataViewModel(ISchmogonClient schmogonClient, SimpleNavigationService navigationService, TrayService trayService)
    {
      _schmogonClient = schmogonClient;
      _navigationService = navigationService;
      _trayService = trayService;
      
      _pokemonSearchReceiver = new MessageReceiver<PokemonSearchMessage>(onPokemonSearched, true);
      _pokemonTypeSelectedSender = new MessageSender<PokemonTypeSelectedMessage>();
      _pokemonAbilitySelectedSender = new MessageSender<PokemonAbilitySelectedMessage>();
      _pokemonMoveSelectedSender = new MessageSender<PokemonMoveSelectedMessage>();
      _movesetSelectedSender = new MessageSender<MovesetSelectedMessage>();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        SchedulePokemonFetch(null);
      }

      setupAppBar();
    }

    #region event handlers

    private void onPokemonSearched(PokemonSearchMessage msg)
    {
      // clear the current pokemon if it exists
      // otherwise we run into stack issues

      PDVM = null;

      Name = msg.Pokemon.Name;

      SchedulePokemonFetch(msg.Pokemon);
    }

    private void onAbilitySelected(AbilityItemViewModel aivm)
    {
      _pokemonAbilitySelectedSender.SendMessage(new PokemonAbilitySelectedMessage(aivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onTypeSelected(TypeItemViewModel tivm)
    {
      _pokemonTypeSelectedSender.SendMessage(new PokemonTypeSelectedMessage(tivm.Type));
      _navigationService.Navigate(ViewModelLocator.TypePath);
    }

    private void onMoveSelected(MoveItemViewModel mivm)
    {
      _pokemonMoveSelectedSender.SendMessage(new PokemonMoveSelectedMessage(mivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
    }

    private void onMovesetSelected(MovesetItemViewModel msivm)
    {
      _movesetSelectedSender.SendMessage(new MovesetSelectedMessage(msivm));
      _navigationService.Navigate((ViewModelLocator.MovesetPath));
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

      MenuItems = new ObservableCollection<MenuItemViewModel> { smogon, bulb };
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

    private void SchedulePokemonFetch(Pokemon pokemon)
    {
      FetchPokemonDataNotifier = NotifyTaskCompletion.Create(fetchPokemonData(pokemon));

      FetchPokemonDataNotifier.PropertyChanged += (sender, args) =>
      {
        // we broked
        if (FetchPokemonDataNotifier == null) return;

        if (FetchPokemonDataNotifier.IsFaulted)
        {
          throw FetchPokemonDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchPokemonData(Pokemon pokemon)
    {
      TrayService.AddJob("fetchdata", "Fetching pokemon data...");

      PokemonData pokemonData;

      try
      {
        pokemonData = await _schmogonClient.GetPokemonDataAsync(pokemon);
      }
      catch (HttpRequestException)
      {
        reloadPokemonData(pokemon);
        return;
      }

      PDVM = new PokemonDataItemViewModel(pokemonData);
      Name = PDVM.Name;

      _pageLocation = pokemon.PageLocation;

      TrayService.RemoveJob("fetchdata");
    }

    private void reloadPokemonData(Pokemon pokemon)
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "I'm sorry, but we couldn't load the pokemon data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "Downloading pokemon data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
      }
      else
      {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        SchedulePokemonFetch(pokemon);
      }
    }

    private void cleanup()
    {
      PDVM = null;
      FetchPokemonDataNotifier = null;
      TrayService.RemoveAllJobs();
    }

  }
}