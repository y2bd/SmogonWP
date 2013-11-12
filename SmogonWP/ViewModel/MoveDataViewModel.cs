﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Moves;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class MoveDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<MoveSearchMessage> _moveSearchReceiver;
    private readonly MessageReceiver<PokemonMoveSelectedMessage> _pokemonMoveSelectedReceiver; 
    private readonly MessageSender<MoveTypeSelectedMessage> _moveTypeSelectedSender; 

    private readonly Stack<MoveDataItemViewModel> _moveStack;

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

    private MoveDataItemViewModel _mdvm;
    public MoveDataItemViewModel MDVM
    {
      get
      {
        return _mdvm;
      }
      set
      {
        if (_mdvm != value)
        {
          _mdvm = value;
          RaisePropertyChanged(() => MDVM);
        }
      }
    }

    private Move _selectedRelatedMove;
    public Move SelectedRelatedMove
    {
      get
      {
        return _selectedRelatedMove;
      }
      set
      {
        if (_selectedRelatedMove != value)
        {
          onRelatedMoveSelected(value);

          _selectedRelatedMove = null;
          RaisePropertyChanged(() => SelectedRelatedMove);
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

    private SolidColorBrush _typeBackgroundBrush;
    public SolidColorBrush TypeBackgroundBrush
    {
      get
      {
        return _typeBackgroundBrush;
      }
      set
      {
        if (_typeBackgroundBrush != value)
        {
          _typeBackgroundBrush = value;
          RaisePropertyChanged(() => TypeBackgroundBrush);
        }
      }
    }			

    #endregion props

    #region commands

    private RelayCommand<CancelEventArgs> _backKeyPressCommand;
    public RelayCommand<CancelEventArgs> BackKeyPressCommand
    {
      get
      {
        return _backKeyPressCommand ??
               (_backKeyPressCommand = new RelayCommand<CancelEventArgs>(onBackKeyPressed));
      }
    }

    private RelayCommand _moveTypeSelected;

    public RelayCommand MoveTypeSelected
    {
      get
      {
        return _moveTypeSelected ??
               (_moveTypeSelected = new RelayCommand(onMoveTypeSelected));
      }
    }
    
    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchMoveDataNotifier { get; private set; }

    #endregion async handlers

    public MoveDataViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _trayService = trayService;

      _moveSearchReceiver = new MessageReceiver<MoveSearchMessage>(onMoveSearched, true);
      _pokemonMoveSelectedReceiver = new MessageReceiver<PokemonMoveSelectedMessage>(onPokemonMoveSelected, true);
      _moveTypeSelectedSender = new MessageSender<MoveTypeSelectedMessage>();

      _moveStack = new Stack<MoveDataItemViewModel>();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(null));
      }

      setupAppBar();
    }

    #region event handlers

    private void onMoveSearched(MoveSearchMessage msg)
    {
      // clear the current move if it exists
      // otherwise we run into stack issues

      MDVM = null;

      Name = msg.Move.Name;

      scheduleMoveFetch(msg.Move);
    }

    private void onPokemonMoveSelected(PokemonMoveSelectedMessage msg)
    {
      // JUST IN CASE
      if (_moveStack != null) _moveStack.Clear();

      MDVM = null;

      Name = msg.Move.Name;

      scheduleMoveFetch(msg.Move);
    }

    private void onRelatedMoveSelected(Move move)
    {
      Name = move.Name;

      scheduleMoveFetch(move);
    }

    private void onBackKeyPressed(CancelEventArgs args)
    {
      if (_moveStack.Count <= 0)
      {
        return;
      }

      // stop from going to the last page
      args.Cancel = true;

      MDVM = _moveStack.Pop();
      Name = MDVM.Name;
    }

    private void onMoveTypeSelected()
    {
      if (MDVM == null || string.IsNullOrEmpty(MDVM.Type)) return;

      Type type;

      // if the parse fails, just ignore it :(
      if (!Enum.TryParse(MDVM.Type, true, out type)) return;

      _moveTypeSelectedSender.SendMessage(new MoveTypeSelectedMessage(type));
      _navigationService.Navigate(ViewModelLocator.TypePath);
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
    
    private void scheduleMoveFetch(Move move)
    {
      FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(move));

      FetchMoveDataNotifier.PropertyChanged += (sender, args) =>
      {
        // we broked
        if (FetchMoveDataNotifier == null) return;

        if (FetchMoveDataNotifier.IsFaulted)
        {
          throw FetchMoveDataNotifier.InnerException;
        }
      };
    }

    private async Task fetchMoveData(Move move)
    {
      TrayService.AddJob("fetchdata", "Fetching move data...");
      
      MoveData moveData;

      try
      {
        moveData = await _schmogonClient.GetMoveDataAsync(move);
      }
      catch (HttpRequestException)
      {
        reloadMoveData(move);
        return;
      }

      // push the current move onto the move stack if there is one
      if (MDVM != null) _moveStack.Push(MDVM);

      MDVM = new MoveDataItemViewModel(moveData);
      Name = MDVM.Name;

      _pageLocation = move.PageLocation;

      Type type;

      if (Enum.TryParse(MDVM.Type, true, out type))
      {
        TypeBackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[type]);
      }

      TrayService.RemoveJob("fetchdata");
    }

    private void reloadMoveData(Move move)
    {
      if (_failedOnce)
      {
        // we failed, give up
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "I'm sorry, but we couldn't load the move data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        _failedOnce = false;
      }
      else if (!NetUtilities.IsNetwork())
      {
        // crafty bastard somehow lost network connectivity midway
        cleanup();

        Name = "Sorry :(";

        MessageBox.Show(
          "Downloading move data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
      }
      else {
        // let's try again
        Debug.WriteLine("Move load failed once.");

        _failedOnce = true;

        scheduleMoveFetch(move);
      }
    }

    private void cleanup()
    {
      MDVM = null;
      FetchMoveDataNotifier = null;
      _moveStack.Clear();
      TrayService.RemoveAllJobs();
    }
  }
}
