using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Google.WebAnalytics;
using Microsoft.WebAnalytics;
using Nito.AsyncEx;
using SchmogonDB.Model.Pokemon;
using SmogonWP.Analytics;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class PokemonSearchViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;

    private readonly MessageSender<ItemSearchedMessage<Pokemon>> _pokemonSearchSender;

    private ICollection<PokemonItemViewModel> _pokemon;

    private string _voicedPokemonName;

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
          onPokemonSelected(value);

          _selectedPokemon = null;
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

    private int _selectedSecondaryTypeFilter;
    public int SelectedSecondaryTypeFilter
    {
      get
      {
        return _selectedSecondaryTypeFilter;
      }
      set
      {
        if (_selectedSecondaryTypeFilter != value)
        {
          _selectedSecondaryTypeFilter = value;
          RaisePropertyChanged(() => SelectedSecondaryTypeFilter);

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

    private TypeItemViewModel _selectedSecondaryType;
    public TypeItemViewModel SelectedSecondaryType
    {
      get
      {
        return _selectedSecondaryType;
      }
      set
      {
        if (_selectedSecondaryType != value)
        {
          _selectedSecondaryType = value;
          RaisePropertyChanged(() => SelectedSecondaryType);
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

    public PokemonSearchViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

      _pokemonSearchSender = new MessageSender<ItemSearchedMessage<Pokemon>>();

      setupFilters();

      schedulePokemonListFetch();

      var msgHandler = new Action<ViewToVmMessage<string, PokemonSearchViewModel>>(onViewMessage);
      MessengerInstance.Register(this, msgHandler);
    }

    private void setupFilters()
    {
      TypeFilters = new List<string> { "none" };

      foreach (var type in Enum.GetNames(typeof(Type)))
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
      // just in case
      if (SelectedTypeFilter == 0) SelectedSecondaryTypeFilter = 0;

      filterPokemon();
    }

    private void onReloadPressed()
    {
      LoadFailed = false;

      schedulePokemonListFetch();
    }

    private void onPokemonSelected(PokemonItemViewModel pivm)
    {
      _pokemonSearchSender.SendMessage(new ItemSearchedMessage<Pokemon>(pivm.Pokemon));
      _navigationService.Navigate(ViewModelLocator.PokemonDataPath);
    }

    private void onViewMessage(ViewToVmMessage<string, PokemonSearchViewModel> msg)
    {
      _voicedPokemonName = msg.Content;

      if (FetchPokemonNotifier.IsSuccessfullyCompleted)
      {
        var pokemon = _pokemon.First(p => p.Name.ToLower().Equals(_voicedPokemonName.ToLower()));

        onPokemonSelected(pokemon);
      }
      else
      {
        FetchPokemonNotifier.PropertyChanged += gotoVoicedPokemonOnLoad;
      }
    }

    private void gotoVoicedPokemonOnLoad(object sender, PropertyChangedEventArgs args)
    {
      if (FetchPokemonNotifier == null || !FetchPokemonNotifier.IsSuccessfullyCompleted) return;

      var pokemon = _pokemon.First(p => p.Name.ToLower().Equals(_voicedPokemonName.ToLower()));

      onPokemonSelected(pokemon);

      FetchPokemonNotifier.PropertyChanged -= gotoVoicedPokemonOnLoad;
    }

    private void filterPokemon()
    {
      var tier = SelectedTierFilter - 1;
      var type = SelectedTypeFilter - 1;
      var secondType = SelectedSecondaryTypeFilter - 1;

      FilteredPokemon = new ObservableCollection<PokemonItemViewModel>(
        _pokemon.Where(m => string.IsNullOrEmpty(Query) || m.Name.ToLower().Contains(Query.ToLower().Trim()))
                .Where(m => tier == -1 || m.Pokemon.Tier == (Tier)tier)
                .Where(m => type == -1 || m.Pokemon.Types.Contains((Type)type))
                .Where(m => secondType == -1 || m.Pokemon.Types.Contains((Type)secondType))
                .OrderBy(m => m.Name));

      SelectedType = type == -1 ? null : new TypeItemViewModel((Type)type);
      SelectedSecondaryType = secondType == -1 ? null : new TypeItemViewModel((Type) secondType);
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
      FilteredPokemon = null;

      try
      {
        var rawPokemon = await _dataService.FetchAllPokemonAsync();

        _pokemon = (from pokemon in rawPokemon
                    select new PokemonItemViewModel(pokemon))
        .ToList();

        FilteredPokemon = new ObservableCollection<PokemonItemViewModel>(_pokemon);

        LoadFailed = false;
      }
      catch (Exception e)
      {
        MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        Debugger.Break();

        LoadFailed = true;

        cleanup();
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
