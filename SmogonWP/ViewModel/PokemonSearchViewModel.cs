using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls.Primitives;
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Pokemon;
using SmogonWP.Services;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.Items;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class PokemonSearchViewModel : ViewModelBase
  {
    private const string PokemonListFilename = "pokemon.txt";

    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;
    private readonly IsolatedStorageService _storageService;

    private bool _failedOnce;

    private ICollection<PokemonItemViewModel> _pokemon;

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

    private string _query;
    public string Query
    {
      get
      {
        return _query;
      }
      set
      {
        if (_query != value)
        {
          _query = value;
          RaisePropertyChanged(() => Query);
        }
      }
    }

    private PokemonItemViewModel _selectedPokemon;
    public PokemonItemViewModel SelectedPokemon
    {
      get
      {
        return _selectedPokemon;
      }
      set
      {
        if (_selectedPokemon != value)
        {
          _selectedPokemon = value;
          RaisePropertyChanged(() => SelectedPokemon);
        }
      }
    }

    private ObservableCollection<PokemonItemViewModel> _filteredPokemon;
    public ObservableCollection<PokemonItemViewModel> FilteredPokemon
    {
      get
      {
        return _filteredPokemon;
      }
      set
      {
        if (_filteredPokemon != value)
        {
          _filteredPokemon = value;
          RaisePropertyChanged(() => FilteredPokemon);
        }
      }
    }

    private bool _loadFailed;
    public bool LoadFailed
    {
      get
      {
        return _loadFailed;
      }
      set
      {
        if (_loadFailed != value)
        {
          _loadFailed = value;
          RaisePropertyChanged(() => LoadFailed);
        }
      }
    }

    private ICollection<string> _typeFilters;
    public ICollection<string> TypeFilters
    {
      get
      {
        return _typeFilters;
      }
      set
      {
        if (_typeFilters != value)
        {
          _typeFilters = value;
          RaisePropertyChanged(() => TypeFilters);
        }
      }
    }

    private ICollection<string> _tierFilters;
    public ICollection<string> TierFilters
    {
      get
      {
        return _tierFilters;
      }
      set
      {
        if (_tierFilters != value)
        {
          _tierFilters = value;
          RaisePropertyChanged(() => TierFilters);
        }
      }
    }

    private int _selectedTypeFilter;
    public int SelectedTypeFilter
    {
      get
      {
        return _selectedTypeFilter;
      }
      set
      {
        if (_selectedTypeFilter != value)
        {
          _selectedTypeFilter = value;
          RaisePropertyChanged(() => SelectedTypeFilter);

          onFilterChanged();
        }
      }
    }

    private int _selectedTierFilter;
    public int SelectedTierFilter
    {
      get
      {
        return _selectedTierFilter;
      }
      set
      {
        if (_selectedTierFilter != value)
        {
          _selectedTierFilter = value;
          RaisePropertyChanged(() => SelectedTierFilter);

          onFilterChanged();
        }
      }
    }

    private TypeItemViewModel _selectedType;
    public TypeItemViewModel SelectedType
    {
      get
      {
        return _selectedType;
      }
      set
      {
        if (_selectedType != value)
        {
          _selectedType = value;
          RaisePropertyChanged(() => SelectedType);
        }
      }
    }

    private string _selectedTier;
    public string SelectedTier
    {
      get
      {
        return _selectedTier;
      }
      set
      {
        if (_selectedTier != value)
        {
          _selectedTier = value;
          RaisePropertyChanged(() => SelectedTier);
        }
      }
    }

    #endregion props

    #region commands

    private RelayCommand<KeyEventArgs> _queryChangedCommand;
    public RelayCommand<KeyEventArgs> QueryChangedCommand
    {
      get
      {
        return _queryChangedCommand ??
               (_queryChangedCommand = new RelayCommand<KeyEventArgs>(onQueryChanged));
      }
    }

    private RelayCommand _reloadCommand;
    public RelayCommand ReloadCommand
    {
      get
      {
        return _reloadCommand ??
               (_reloadCommand = new RelayCommand(onReloadPressed));
      }
    }

    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchPokemonNotifier { get; private set; }

    #endregion

    public PokemonSearchViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, IsolatedStorageService storageService, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _storageService = storageService;
      _trayService = trayService;

      setupFilters();

      schedulePokemonListFetch();
    }

    private void setupFilters()
    {
      TypeFilters = new List<string> { "none" };

      foreach (var type in Enum.GetNames(typeof(Type)).Where(t => !t.Equals("Fairy")))
      {
        TypeFilters.Add(type.ToLower());
      }

      TierFilters = new List<string> { "none" };

      foreach (var tier in Enum.GetValues(typeof(Tier)).Cast<Tier>())
      {
        TierFilters.Add(TierUtils.GetName(tier).ToLower());
      }
    }

    private void onQueryChanged(KeyEventArgs args)
    {
      if (_pokemon == null || Query == null) return;
      if (args.Key != Key.Enter) return;

      filterPokemon();
    }

    private void onFilterChanged()
    {
      filterPokemon();
    }

    private void onReloadPressed()
    {
      LoadFailed = false;

      schedulePokemonListFetch();
    }

    private void filterPokemon()
    {
      var tier = SelectedTierFilter - 1;
      var type = SelectedTypeFilter - 1;

      FilteredPokemon = new ObservableCollection<PokemonItemViewModel>(
        _pokemon.Where(m => string.IsNullOrEmpty(Query) || m.Name.ToLower().Contains(Query.ToLower().Trim()))
                .Where(m => tier == -1 || m.Pokemon.Tier == (Tier)tier)
                .Where(m => type == -1 || m.Pokemon.Types.Contains((Type)type))
                .OrderBy(m => m.Name));

      SelectedType = type == -1 ? null : new TypeItemViewModel((Type)type);
      SelectedTier = tier == -1 ? null : Enum.GetName(typeof(Tier), (Tier)tier);
    }

    private void schedulePokemonListFetch()
    {
      FetchPokemonNotifier = NotifyTaskCompletion.Create(fetchPokemon());

      FetchPokemonNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchPokemonNotifier == null) return;

        if (FetchPokemonNotifier.IsFaulted)
        {
          throw FetchPokemonNotifier.InnerException;
        }
      };
    }

    private async Task fetchPokemon()
    {
      TrayService.AddJob("pokefetch", "Fetching pokemon");

      // hehe raw pokemon
      var rawPokemon = await fetchPokemonFromStorage();

      // no pokemon in cache :(
      if (rawPokemon == null)
      {
        try
        {
          rawPokemon = await _schmogonClient.GetAllPokemonAsync();
        }
        catch (HttpRequestException)
        {
          reloadPokemon();
          return;
        }

        await cacheMoves();
      }

      _pokemon = (from pokemon in rawPokemon
                  select new PokemonItemViewModel(pokemon))
        .ToList();

      FilteredPokemon = new ObservableCollection<PokemonItemViewModel>(_pokemon);

      LoadFailed = false;

      TrayService.RemoveJob("pokefetch");
    }

    private async Task<IEnumerable<Pokemon>> fetchPokemonFromStorage()
    {
      IEnumerable<Pokemon> pokeCache = null;

      if (await _storageService.FileExistsAsync(PokemonListFilename))
      {
        var cereal = await _storageService.ReadStringFromFileAsync(PokemonListFilename);

        pokeCache = await _schmogonClient.DeserializePokemonListAsync(cereal);
      }

      return pokeCache;
    }

    private async Task cacheMoves()
    {
      var cereal = await _schmogonClient.SerializePokemonListAsync();

      await _storageService.WriteStringToFileAsync(PokemonListFilename, cereal);
    }

    private void reloadPokemon()
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();

        MessageBox.Show(
          "I'm sorry, but we couldn't load the Pokemon data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;

        LoadFailed = true;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        MessageBox.Show(
          "Downloading move data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);

        LoadFailed = true;
      }
      else
      {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        schedulePokemonListFetch();
      }
    }

    private void cleanup()
    {
      _pokemon = null;
      FilteredPokemon = null;
      FetchPokemonNotifier = null;
      Query = null;
      TrayService.RemoveAllJobs();
    }
  }
}
