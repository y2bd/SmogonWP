using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Nito.AsyncEx;
using Schmogon.Data.Abilities;
using Schmogon.Data.Pokemon;
using SchmogonDB.Model;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmogonWP.ViewModel
{
  public class PokemonDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly IDataLoadingService _dataService;
    private readonly SimpleNavigationService _navigationService;

    private readonly MessageReceiver<ItemSearchedMessage<Pokemon>> _pokemonSearchReceiver;
    private readonly MessageSender<PokemonTypeSelectedMessage> _pokemonTypeSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Ability>> _pokemonAbilitySelectedSender;
    private readonly MessageSender<ItemSelectedMessage<TypedMove>> _pokemonMoveSelectedSender;
    private readonly MessageSender<ItemSelectedMessage<MovesetItemViewModel>> _movesetSelectedSender; 
    
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

    private TypedMoveItemViewModel _selectedMove;
    public TypedMoveItemViewModel SelectedMove
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

    public PokemonDataViewModel(IDataLoadingService dataService, SimpleNavigationService navigationService, TrayService trayService)
    {
      _dataService = dataService;
      _navigationService = navigationService;
      _trayService = trayService;

      _pokemonSearchReceiver = new MessageReceiver<ItemSearchedMessage<Pokemon>>(onPokemonSearched, true);
      _pokemonTypeSelectedSender = new MessageSender<PokemonTypeSelectedMessage>();
      _pokemonAbilitySelectedSender = new MessageSender<ItemSelectedMessage<Ability>>();
      _pokemonMoveSelectedSender = new MessageSender<ItemSelectedMessage<TypedMove>>();
      _movesetSelectedSender = new MessageSender<ItemSelectedMessage<MovesetItemViewModel>>();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        SchedulePokemonFetch(null);
      }

      setupAppBar();
    }

    #region event handlers

    private void onPokemonSearched(ItemSearchedMessage<Pokemon> msg)
    {
      // clear the current pokemon if it exists
      // otherwise we run into stack issues

      PDVM = null;

      Name = msg.Item.Name;

      SchedulePokemonFetch(msg.Item);
    }

    private void onAbilitySelected(AbilityItemViewModel aivm)
    {
      _pokemonAbilitySelectedSender.SendMessage(new ItemSelectedMessage<Ability>(aivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onTypeSelected(TypeItemViewModel tivm)
    {
      _pokemonTypeSelectedSender.SendMessage(new PokemonTypeSelectedMessage(tivm.Type));
      _navigationService.Navigate(ViewModelLocator.TypePath);
    }

    private void onMoveSelected(TypedMoveItemViewModel mivm)
    {
      _pokemonMoveSelectedSender.SendMessage(new ItemSelectedMessage<TypedMove>(mivm.TypedMove));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
    }

    private void onMovesetSelected(MovesetItemViewModel msivm)
    {
      _movesetSelectedSender.SendMessage(new ItemSelectedMessage<MovesetItemViewModel>(msivm));
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
      var name = toTitleCase(Name);

      // dammit ho-oh!
      if (name.Contains("-") && !name.Equals("Ho-oh"))
      {
        // take every character before the hyphen, bulbapedia doesn't like forme suffixes
        name = string.Join("", name.TakeWhile(c => c != '-'));
      }

      var wbt = new WebBrowserTask
      {
        Uri = new Uri(BulbaPrefix + Uri.EscapeDataString(name))
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
        pokemonData = await _dataService.FetchPokemonDataAsync(pokemon);
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

      PDVM = new PokemonDataItemViewModel(pokemonData);
      Name = PDVM.Name;

      _pageLocation = pokemon.PageLocation;

      TrayService.RemoveJob("fetchdata");
    }
    
    private void cleanup()
    {
      PDVM = null;
      FetchPokemonDataNotifier = null;
      TrayService.RemoveAllJobs();
    }

  }
}