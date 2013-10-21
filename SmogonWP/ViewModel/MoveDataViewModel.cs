using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Nito.AsyncEx;
using Schmogon;
using Schmogon.Data.Moves;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class MoveDataViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<MoveSearchMessage> _moveSearchReceiver; 

    #region props

    private string _name;
    public string Name
    {
      get
      {
        return _name.ToLower();
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

    #region async handlers

    public INotifyTaskCompletion FetchMoveDataNotifier { get; private set; }

    #endregion async handlers

    public MoveDataViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _trayService = trayService;

      _moveSearchReceiver = new MessageReceiver<MoveSearchMessage>(onMoveSearched, true);

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(null));
      }
    }

    private async Task fetchMoveData(Move move)
    {
      TrayService.AddJob("fetchdata", "Fetching move data...");

      var moveData = await _schmogonClient.GetMoveDataAsync(move);

      MDVM = new MoveDataItemViewModel(moveData);
      Name = MDVM.Name;

      TrayService.RemoveJob("fetchdata");
    }

    private void onMoveSearched(MoveSearchMessage msg)
    {
      Name = msg.Move.Name;

      FetchMoveDataNotifier = NotifyTaskCompletion.Create(fetchMoveData(msg.Move));
    }
  }
}
