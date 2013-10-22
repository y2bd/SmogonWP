using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using Schmogon;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class MoveSearchViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageSender<MoveSearchMessage> _moveSearchSender; 

    private List<MoveItemViewModel> _moves;

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

    private MoveItemViewModel _selectedMove;
    public MoveItemViewModel SelectedMove
    {
      get
      {
        return _selectedMove;
      }
      set
      {
        if (_selectedMove != value)
        {
          onMoveSelected(value);

          _selectedMove = null;
          RaisePropertyChanged(() => SelectedMove);
        }
      }
    }			

    private ObservableCollection<MoveItemViewModel> _filteredMoves;
    public ObservableCollection<MoveItemViewModel> FilteredMoves
    {
      get
      {
        return _filteredMoves;
      }
      set
      {
        if (_filteredMoves != value)
        {
          _filteredMoves = value;
          RaisePropertyChanged(() => FilteredMoves);
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
    
    #endregion commands

    #region async handlers

    public INotifyTaskCompletion FetchMovesNotifier { get; private set; }

    #endregion

    public MoveSearchViewModel(SimpleNavigationService navigationService, ISchmogonClient schmogonClient, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonClient = schmogonClient;
      _trayService = trayService;

      _moveSearchSender = new MessageSender<MoveSearchMessage>();

      scheduleDataFetch();
    }

    private async Task fetchMoves()
    {
      TrayService.AddJob("movefetch", "Fetching moves");

      var rawMoves = (await _schmogonClient.GetAllMovesAsync()).ToList();

      _moves = (from rawMove in rawMoves
                select new MoveItemViewModel(rawMove))
        .ToList();

      FilteredMoves = new ObservableCollection<MoveItemViewModel>(_moves);

      TrayService.RemoveJob("movefetch");
    }

    private void onFilterChanged(KeyEventArgs args)
    {
      if (_moves == null || Filter == null) return;
      if (args.Key != Key.Enter) return;

      if (string.IsNullOrWhiteSpace(Filter)) FilteredMoves = new ObservableCollection<MoveItemViewModel>(_moves);
      
      FilteredMoves = new ObservableCollection<MoveItemViewModel>(
        _moves.Where(
          m => m.Name.ToLower().Contains(Filter.ToLower().Trim())
        ).OrderBy(m => m.Name)
      );
    }

    private void onMoveSelected(MoveItemViewModel mivm)
    {
      _moveSearchSender.SendMessage(new MoveSearchMessage(mivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataViewModel);
    }

    private void scheduleDataFetch()
    {
      FetchMovesNotifier = NotifyTaskCompletion.Create(fetchMoves());

      FetchMovesNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchMovesNotifier.IsFaulted)
        {
          throw FetchMovesNotifier.Exception;
        }
      };
    }
  }
}
