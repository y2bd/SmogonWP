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
using Schmogon.Data.Items;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class ItemDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<ItemSearchedMessage<Item>> _itemSearchReceiver;
    private readonly MessageReceiver<ItemSelectedMessage<Item>> _pokemonItemSelectedReceiver; 
    
    // if a network request fails, we'll try again one more time
    // otherwise we'll give up and tell the user
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

    public ItemDataViewModel(ISchmogonClient schmogonClient, TrayService trayService)
    {
      _schmogonClient = schmogonClient;
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
        itemData = await _schmogonClient.GetItemDataAsync(item);
      }
      catch (HttpRequestException)
      {
        reloadItemData(item);
        return;
      }

      IDVM = new ItemDataItemViewModel(itemData);
      Name = IDVM.Name;

      _pageLocation = item.PageLocation;

      TrayService.RemoveJob("fetchdata");
    }

    private void reloadItemData(Item item)
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "I'm sorry, but we couldn't load the item data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "Downloading item data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
      }
      else {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        scheduleItemFetch(item);
      }
    }

    private void cleanup()
    {
      IDVM = null;
      FetchItemDataNotifier = null;
      TrayService.RemoveAllJobs();
    }
  }
}