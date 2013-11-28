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
using Schmogon.Data.Items;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class ItemSearchViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;

    private readonly MessageSender<ItemSearchedMessage<Item>> _itemSearchSender;
    
    private List<ItemItemViewModel> _items;

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

    private ItemItemViewModel _selectedItem;
    public ItemItemViewModel SelectedItem
    {
      get
      {
        return _selectedItem;
      }
      set
      {
        if (_selectedItem != value)
        {
          onItemSelected(value);

          _selectedItem = null;
          RaisePropertyChanged(() => SelectedItem);
        }
      }
    }

    private ObservableCollection<ItemItemViewModel> _filteredItems;
    public ObservableCollection<ItemItemViewModel> FilteredItems
    {
      get
      {
        return _filteredItems;
      }
      set
      {
        if (_filteredItems != value)
        {
          _filteredItems = value;
          RaisePropertyChanged(() => FilteredItems);
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

    public INotifyTaskCompletion FetchItemsNotifier { get; private set; }

    #endregion

    public ItemSearchViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

      _itemSearchSender = new MessageSender<ItemSearchedMessage<Item>>();

      scheduleItemListFetch();
    }

    private void onFilterChanged(KeyEventArgs args)
    {
      if (_items == null || Filter == null) return;
      if (args.Key != Key.Enter) return;

      if (string.IsNullOrWhiteSpace(Filter)) FilteredItems = new ObservableCollection<ItemItemViewModel>(_items);

      FilteredItems = new ObservableCollection<ItemItemViewModel>(
        _items.Where(
          m => m.Name.ToLower().Contains(Filter.ToLower().Trim())
        ).OrderBy(m => m.Name)
      );
    }

    private void onItemSelected(ItemItemViewModel iivm)
    {
      _itemSearchSender.SendMessage(new ItemSearchedMessage<Item>(iivm.Item));
      _navigationService.Navigate(ViewModelLocator.ItemDataPath);
    }

    private void onReloadPressed()
    {
      LoadFailed = false;

      scheduleItemListFetch();
    }

    private void scheduleItemListFetch()
    {
      FetchItemsNotifier = NotifyTaskCompletion.Create(fetchItems());

      FetchItemsNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchItemsNotifier == null) return;

        if (FetchItemsNotifier.IsFaulted)
        {
          throw FetchItemsNotifier.InnerException;
        }
      };
    }

    private async Task fetchItems()
    {
      FilteredItems = null;

      try
      {
        var rawItems = await _dataService.FetchAllItemsAsync();

        _items = (from item in rawItems
                  select new ItemItemViewModel(item))
        .ToList();

        FilteredItems = new ObservableCollection<ItemItemViewModel>(_items);

        LoadFailed = false;
      }
      catch (Exception e)
      {
        if (!NetUtilities.IsNetwork())
        {
          MessageBox.Show(
          "Downloading item data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
        }
        else
        {
          MessageBox.Show(
          "I'm sorry, but we couldn't load the item data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);
        }

        Debugger.Break();

        LoadFailed = true;

        cleanup();
      }
    }

    private void cleanup()
    {
      _items = null;
      FilteredItems = null;
      FetchItemsNotifier = null;
      Filter = null;
      TrayService.RemoveAllJobs();
    }
  }
}
