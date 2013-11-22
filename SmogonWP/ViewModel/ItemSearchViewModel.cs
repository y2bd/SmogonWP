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
    private const string ItemListFilename = "items.txt";

    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;
    private readonly IsolatedStorageService _storageService;

    private readonly MessageSender<ItemSearchedMessage<Item>> _itemSearchSender;

    private bool _failedOnce;

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

    public ItemSearchViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, TrayService trayService, IsolatedStorageService storageService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _trayService = trayService;
      _storageService = storageService;

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
      TrayService.AddJob("itemfetch", "Fetching items");

      var rawItems = await fetchItemsFromStorage();

      // if we couldn't get items from the cache...
      if (rawItems == null)
      {
        Debug.WriteLine("Reading items from internetland!");

        try
        {
          rawItems = (await _schmogonClient.GetAllItemsAsync()).ToList();
        }
        catch (HttpRequestException)
        {
          reloadItems();
          return;
        }

        // cache 'em for next time
        await cacheItems();
      }
      else
      {
        Debug.WriteLine("Reading items from fileland!");
      }

      _items = (from rawItem in rawItems
                select new ItemItemViewModel(rawItem))
        .ToList();

      FilteredItems = new ObservableCollection<ItemItemViewModel>(_items);

      TrayService.RemoveJob("itemfetch");
    }

    private async Task<IEnumerable<Item>> fetchItemsFromStorage()
    {
      IEnumerable<Item> itemCache = null;

      if (await _storageService.FileExistsAsync(ItemListFilename))
      {
        var cereal = await _storageService.ReadStringFromFileAsync(ItemListFilename);

        itemCache = (await _schmogonClient.DeserializeItemListAsync(cereal));
      }

      return itemCache;
    }

    private async Task cacheItems()
    {
      var cereal = await _schmogonClient.SerializeItemListAsync();

      await _storageService.WriteStringToFileAsync(ItemListFilename, cereal);
    }

    private void reloadItems()
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();
        
        MessageBox.Show(
          "I'm sorry, but we couldn't load the item data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;

        LoadFailed = true;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        MessageBox.Show(
          "Downloading item data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);

        LoadFailed = true;
      }
      else
      {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        scheduleItemListFetch();
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
