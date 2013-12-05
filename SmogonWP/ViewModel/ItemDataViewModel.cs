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
using Schmogon.Data.Items;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class ItemDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly IDataLoadingService _dataService;

    private readonly MessageReceiver<ItemSearchedMessage<Item>> _itemSearchReceiver;
    private readonly MessageReceiver<ItemSelectedMessage<Item>> _pokemonItemSelectedReceiver; 
    
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

    private ItemDataItemViewModel _idvm;
    public ItemDataItemViewModel IDVM
    {
      get
      {
        return _idvm;
      }
      set
      {
        if (_idvm != value)
        {
          _idvm = value;
          RaisePropertyChanged(() => IDVM);
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

    public INotifyTaskCompletion FetchItemDataNotifier { get; private set; }

    #endregion async handlers

    public ItemDataViewModel(IDataLoadingService dataService, TrayService trayService)
    {
      _dataService = dataService;
      _trayService = trayService;

      _itemSearchReceiver = new MessageReceiver<ItemSearchedMessage<Item>>(onItemSearched, true);
      _pokemonItemSelectedReceiver = new MessageReceiver<ItemSelectedMessage<Item>>(onPokemonItemSelected, true);
      
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchItemDataNotifier = NotifyTaskCompletion.Create(fetchItemData(null));
      }

      setupAppBar();
    }

    #region event handlers

    private void onItemSearched(ItemSearchedMessage<Item> msg)
    {
      // clear the current item if it exists
      // otherwise we run into stack issues

      IDVM = null;

      Name = msg.Item.Name;

      scheduleItemFetch(msg.Item);
    }

    private void onPokemonItemSelected(ItemSelectedMessage<Item> msg)
    {
      IDVM = null;

      Name = msg.Item.Name;

      scheduleItemFetch(msg.Item);
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

      MenuItems = new ObservableCollection<MenuItemViewModel> {smogon, bulb};
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
    
    private void scheduleItemFetch(Item item)
    {
      FetchItemDataNotifier = NotifyTaskCompletion.Create(fetchItemData(item));

      FetchItemDataNotifier.PropertyChanged += (sender, args) =>
      {
        // we broked
        if (FetchItemDataNotifier == null) return;

        if (FetchItemDataNotifier.IsFaulted)
        {
          throw FetchItemDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchItemData(Item item)
    {
      TrayService.AddJob("fetchdata", "Fetching item data...");
      
      ItemData itemData;

      try
      {
        itemData = await _dataService.FetchItemDataAsync(item);
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

      IDVM = new ItemDataItemViewModel(itemData);
      Name = IDVM.Name;

      _pageLocation = item.PageLocation;

      TrayService.RemoveJob("fetchdata");
    }
    
    private void cleanup()
    {
      IDVM = null;
      FetchItemDataNotifier = null;
      TrayService.RemoveAllJobs();
    }
  }
}