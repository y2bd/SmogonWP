using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Moves;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class MoveDataViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<MoveSearchMessage> _moveSearchReceiver;

    private readonly Stack<MoveDataItemViewModel> _moveStack;

    // if a network request fails, we'll try again one more time
    // otherwise we'll give up and tell the user
    private bool _failedOnce;

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

      _moveStack = new Stack<MoveDataItemViewModel>();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(null));
      }
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

    #endregion event handlers

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
