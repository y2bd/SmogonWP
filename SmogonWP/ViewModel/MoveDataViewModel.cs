using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Nito.AsyncEx;
using SchmogonDB.Model.Moves;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class MoveDataViewModel : ViewModelBase
  {
    private const string SmogonPrefix = "http://www.smogon.com";
    private const string BulbaPrefix = "http://bulbapedia.bulbagarden.net/wiki/";

    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;
    private readonly TombstoneService _tombstoneService; 

    private readonly MessageReceiver<ItemSearchedMessage<Move>> _moveSearchReceiver;
    private readonly MessageReceiver<ItemSelectedMessage<Move>> _pokemonMoveSelectedReceiver; 
    private readonly MessageSender<ItemSelectedMessage<Type>> _moveTypeSelectedSender; 

    private readonly Stack<MoveDataItemViewModel> _mdivmStack;
    
    private string _pageLocation;

    // for serial
    private Stack<Move> _moveStack; 

    #region props

    private string _name = string.Empty;
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

    public MoveDataViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService, TombstoneService tombstoneService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;
      _tombstoneService = tombstoneService;

      _mdivmStack = new Stack<MoveDataItemViewModel>();
      _moveStack = new Stack<Move>();

      _moveSearchReceiver = new MessageReceiver<ItemSearchedMessage<Move>>(onMoveSearched, true);
      _pokemonMoveSelectedReceiver = new MessageReceiver<ItemSelectedMessage<Move>>(onPokemonMoveSelected, true);
      _moveTypeSelectedSender = new MessageSender<ItemSelectedMessage<Type>>();
      
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(null));
      }

      setupAppBar();

      MessengerInstance.Register(this, new Action<TombstoneMessage<MoveDataViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<MoveDataViewModel>>(m => restore()));
    }

    #region event handlers

    private void onMoveSearched(ItemSearchedMessage<Move> msg)
    {
      // clear the current move if it exists
      // otherwise we run into stack issues

      MDVM = null;

      Name = msg.Item.Name;

      scheduleMoveFetch(msg.Item);
    }

    private void onPokemonMoveSelected(ItemSelectedMessage<Move> msg)
    {
      // JUST IN CASE
      if (_mdivmStack != null) _mdivmStack.Clear();

      MDVM = null;

      Name = msg.Item.Name;

      scheduleMoveFetch(msg.Item);
    }

    private void onRelatedMoveSelected(Move move)
    {
      Name = move.Name;

      scheduleMoveFetch(move);
    }

    private void onBackKeyPressed(CancelEventArgs args)
    {
      if (_mdivmStack.Count <= 0)
      {
        return;
      }

      // stop from going to the last page
      args.Cancel = true;

      MDVM = _mdivmStack.Pop();
      Name = MDVM.Name;

      _moveStack.Pop();
    }

    private void onMoveTypeSelected()
    {
      if (MDVM == null || string.IsNullOrEmpty(MDVM.Type)) return;

      Type type;

      // if the parse fails, just ignore it :(
      if (!Enum.TryParse(MDVM.Type, true, out type)) return;

      _moveTypeSelectedSender.SendMessage(new ItemSelectedMessage<Type>(type));
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
      // damn hidden power
      var pl = _pageLocation.Contains("hidden_power") ? "/bw/moves/hidden_power" : _pageLocation;

      var wbt = new WebBrowserTask
      {
        Uri = new Uri(SmogonPrefix + pl)
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
      
      _moveStack.Push(move);

      MoveData moveData;

      try
      {
        moveData = await _dataService.FetchMoveDataAsync(move);
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

      // push the current move onto the move stack if there is one
      if (MDVM != null)
      {
        _mdivmStack.Push(MDVM);
      }

      MDVM = new MoveDataItemViewModel(moveData);
      Name = MDVM.Name;

      _pageLocation = move.PageLocation;
      
      TypeBackgroundBrush = new SolidColorBrush(TypeItemViewModel.TypeColors[moveData.Stats.Type]);

      TrayService.RemoveJob("fetchdata");
    }
    
    private void cleanup()
    {
      MDVM = null;
      FetchMoveDataNotifier = null;
      _mdivmStack.Clear();
      TrayService.RemoveAllJobs();
    }

    private async void tombstone()
    {
      // we're only going to store the most recent move
      // sorry low-end devices
      
      if (_moveStack != null && _moveStack.Count > 0)
      {
        var toSave = _moveStack.Peek();
        await _tombstoneService.Store("ts_move", toSave);
      }

      //await _tombstoneService.Save();
    }

    private async void restore()
    {
      if (MDVM != null) return;

      var loaded = await _tombstoneService.Load<Move>("ts_move");

      if (loaded != null)
      {
        Name = loaded.Name;
        scheduleMoveFetch(loaded);
      }
    }
  }
}
