using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
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

    private readonly SimpleNavigationService _navigationService;
    private readonly DataLoadingService _dataService;

    private readonly MessageSender<ItemSearchedMessage<Ability>> _abilitySearchSender;

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

    public AbilitySearchViewModel(SimpleNavigationService navigationService, DataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

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
      FilteredAbilities = null;

      try
      {
        var rawAbilities = await _dataService.FetchAllAbilitiesAsync();

        _abilities = (from ability in rawAbilities
                    select new AbilityItemViewModel(ability))
        .ToList();

        FilteredAbilities = new ObservableCollection<AbilityItemViewModel>(_abilities);

        LoadFailed = false;
      }
      catch (Exception e)
      {
        if (!NetUtilities.IsNetwork())
        {
          MessageBox.Show(
          "Downloading ability data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
        }
        else
        {
          MessageBox.Show(
          "I'm sorry, but we couldn't load the ability data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);
        }

        Debugger.Break();

        LoadFailed = true;

        cleanup();
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
