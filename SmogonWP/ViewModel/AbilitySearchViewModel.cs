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
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Abilities;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class AbilitySearchViewModel : ViewModelBase
  {
    private const string AbilityListFilename = "abilities.txt";

    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;
    private readonly IsolatedStorageService _storageService;

    private readonly MessageSender<ItemSearchedMessage<Ability>> _abilitySearchSender;

    private bool _failedOnce;

    private List<AbilityItemViewModel> _abilities;

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

    private ObservableCollection<AbilityItemViewModel> _filteredAbilities;
    public ObservableCollection<AbilityItemViewModel> FilteredAbilities
    {
      get
      {
        return _filteredAbilities;
      }
      set
      {
        if (_filteredAbilities != value)
        {
          _filteredAbilities = value;
          RaisePropertyChanged(() => FilteredAbilities);
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

    #endregion props

    #region commands

    private RelayCommand<KeyEventArgs> _filterChangedCommand;
    public RelayCommand<KeyEventArgs> FilterChangedCommand
    {
      get
      {
        return _filterChangedCommand ??
               (_filterChangedCommand = new RelayCommand<KeyEventArgs>(onFilterChanged));
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

    public INotifyTaskCompletion FetchAbilitiesNotifier { get; private set; }

    #endregion

    public AbilitySearchViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, TrayService trayService, IsolatedStorageService storageService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _trayService = trayService;
      _storageService = storageService;

      _abilitySearchSender = new MessageSender<ItemSearchedMessage<Ability>>();

      scheduleAbilityListFetch();
    }
    
    private void onFilterChanged(KeyEventArgs args)
    {
      if (_abilities == null || Filter == null) return;
      if (args.Key != Key.Enter) return;

      if (string.IsNullOrWhiteSpace(Filter)) FilteredAbilities = new ObservableCollection<AbilityItemViewModel>(_abilities);
      
      FilteredAbilities = new ObservableCollection<AbilityItemViewModel>(
        _abilities.Where(
          m => m.Name.ToLower().Contains(Filter.ToLower().Trim())
        ).OrderBy(m => m.Name)
      );
    }

    private void onAbilitySelected(AbilityItemViewModel mivm)
    {
      _abilitySearchSender.SendMessage(new ItemSearchedMessage<Ability>(mivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onReloadPressed()
    {
      LoadFailed = false;

      scheduleAbilityListFetch();
    }

    private void scheduleAbilityListFetch()
    {
      FetchAbilitiesNotifier = NotifyTaskCompletion.Create(fetchAbilities());

      FetchAbilitiesNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchAbilitiesNotifier == null) return;

        if (FetchAbilitiesNotifier.IsFaulted)
        {
          throw FetchAbilitiesNotifier.InnerException;
        }
      };
    }

    private async Task fetchAbilities()
    {
      TrayService.AddJob("abilityfetch", "Fetching abilities");

      var rawAbilities = await fetchAbilitiesFromStorage();

      // if we couldn't get abilities from the cache...
      if (rawAbilities == null)
      {
        Debug.WriteLine("Reading abilities from internetland!");

        try
        {
          rawAbilities = (await _schmogonClient.GetAllAbilitiesAsync()).ToList();
        }
        catch (HttpRequestException)
        {
          reloadAbilities();
          return;
        }

        // cache 'em for next time
        await cacheAbilities();
      }
      else
      {
        Debug.WriteLine("Reading abilities from fileland!");
      }

      _abilities = (from rawAbility in rawAbilities
                select new AbilityItemViewModel(rawAbility))
        .ToList();

      FilteredAbilities = new ObservableCollection<AbilityItemViewModel>(_abilities);

      TrayService.RemoveJob("abilityfetch");
    }

    private async Task<IEnumerable<Ability>> fetchAbilitiesFromStorage()
    {
      IEnumerable<Ability> abilityCache = null;

      if (await _storageService.FileExistsAsync(AbilityListFilename))
      {
        var cereal = await _storageService.ReadStringFromFileAsync(AbilityListFilename);

        abilityCache = (await _schmogonClient.DeserializeAbilityListAsync(cereal));
      }

      return abilityCache;
    }

    private async Task cacheAbilities()
    {
      var cereal = await _schmogonClient.SerializeAbilityListAsync();

      await _storageService.WriteStringToFileAsync(AbilityListFilename, cereal);
    }

    private void reloadAbilities()
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();
        
        MessageBox.Show(
          "I'm sorry, but we couldn't load the ability data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;

        LoadFailed = true;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        MessageBox.Show(
          "Downloading ability data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);

        LoadFailed = true;
      }
      else
      {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        scheduleAbilityListFetch();
      }
    }

    private void cleanup()
    {
      _abilities = null;
      FilteredAbilities = null;
      FetchAbilitiesNotifier = null;
      Filter = null;
      TrayService.RemoveAllJobs();
    }
  }
}
