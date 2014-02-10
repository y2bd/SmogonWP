using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using SchmogonDB.Model.Moves;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class MoveSearchViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly IDataLoadingService _dataService;

    private readonly MessageSender<ItemSearchedMessage<Move>> _moveSearchSender;

    private List<MoveItemViewModel> _moves;

    private string _voicedMoveName;

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

    private string _query;
    public string Query
    {
      get
      {
        return _query;
      }
      set
      {
        if (_query != value)
        {
          _query = value;
          RaisePropertyChanged(() => Query);
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

    private ObservableCollection<string> _filters;
    public ObservableCollection<string> Filters
    {
      get
      {
        return _filters;
      }
      set
      {
        if (_filters != value)
        {
          _filters = value;
          RaisePropertyChanged(() => Filters);
        }
      }
    }

    private int _selectedFilter;
    public int SelectedFilter
    {
      get
      {
        return _selectedFilter;
      }
      set
      {
        if (_selectedFilter != value)
        {
          _selectedFilter = value;
          RaisePropertyChanged(() => SelectedFilter);

          onFilterChanged();
        }
      }
    }

    private TypeItemViewModel _typeDisplay;
    public TypeItemViewModel TypeDisplay
    {
      get
      {
        return _typeDisplay;
      }
      set
      {
        if (_typeDisplay != value)
        {
          _typeDisplay = value;
          RaisePropertyChanged(() => TypeDisplay);
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
               (_filterChangedCommand = new RelayCommand<KeyEventArgs>(onQueryChanged));
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

    public INotifyTaskCompletion FetchMovesNotifier { get; private set; }

    #endregion

    public MoveSearchViewModel(SimpleNavigationService navigationService, IDataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

      _moveSearchSender = new MessageSender<ItemSearchedMessage<Move>>();

      Filters = new ObservableCollection<string> {"none"};

      foreach (var type in Enum.GetNames(typeof (Type)))
      {
        Filters.Add(type.ToLower());
      }

      scheduleMoveListFetch();

      var msgHandler = new Action<ViewToVmMessage<string, MoveSearchViewModel>>(onViewMessage);
      MessengerInstance.Register(this, msgHandler);
    }
    
    private void onQueryChanged(KeyEventArgs args)
    {
      if (_moves == null || Query == null) return;
      if (args.Key != Key.Enter) return;

      if (string.IsNullOrWhiteSpace(Query)) FilteredMoves = new ObservableCollection<MoveItemViewModel>(_moves);
      else
      {
        FilteredMoves = new ObservableCollection<MoveItemViewModel>(
          _moves.Where(
            m => m.Name.ToLower().Contains(Query.ToLower().Trim())
            ).OrderBy(m => m.Name)
          );
      }
    }

    private void onFilterChanged()
    {
      var type = SelectedFilter - 1;

      FilteredMoves = new ObservableCollection<MoveItemViewModel>(
        _moves.Where(m => type == -1 || m.Type.Type == (Type)type)
              .OrderBy(m => m.Name));

      Query = string.Empty;

      TypeDisplay = type == -1 ? null : new TypeItemViewModel((Type) type);
    }

    private void onMoveSelected(MoveItemViewModel mivm)
    {
      _moveSearchSender.SendMessage(new ItemSearchedMessage<Move>(mivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath + "?move=" + Uri.EscapeDataString(mivm.Name));
    }

    private void onReloadPressed()
    {
      LoadFailed = false;

      scheduleMoveListFetch();
    }

    private void onViewMessage(ViewToVmMessage<string, MoveSearchViewModel> msg)
    {
      _voicedMoveName = msg.Content;

      if (FetchMovesNotifier.IsSuccessfullyCompleted)
      {
        var move = _moves.First(m => m.Name.ToLower().Equals(_voicedMoveName.ToLower()));

        onMoveSelected(move);
      }
      else
      {
        FetchMovesNotifier.PropertyChanged += gotoVoicedMoveOnLoad;
      }
    }

    private void gotoVoicedMoveOnLoad(object sender, PropertyChangedEventArgs args)
    {
      if (FetchMovesNotifier == null || !FetchMovesNotifier.IsSuccessfullyCompleted) return;

      var pokemon = _moves.First(m => m.Name.ToLower().Equals(_voicedMoveName.ToLower()));

      onMoveSelected(pokemon);

      FetchMovesNotifier.PropertyChanged -= gotoVoicedMoveOnLoad;
    }

    private void scheduleMoveListFetch()
    {
      FetchMovesNotifier = NotifyTaskCompletion.Create(fetchMoves());

      FetchMovesNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchMovesNotifier == null) return;

        if (FetchMovesNotifier.IsFaulted)
        {
          throw FetchMovesNotifier.InnerException;
        }
      };
    }

    private async Task fetchMoves()
    {
      FilteredMoves = null;
      
      try
      {
        var rawMoves = await _dataService.FetchAllMovesAsync();

        _moves = (from rawMove in rawMoves
                  select new MoveItemViewModel(rawMove))
        .OrderBy(m => m.Name)
        .ToList();

        FilteredMoves = new ObservableCollection<MoveItemViewModel>(_moves);

        LoadFailed = false;
      }
      catch (Exception)
      {
        MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        Debugger.Break();

        LoadFailed = true;

        cleanup();
      }
    }
    
    private void cleanup()
    {
      _moves = null;
      FilteredMoves = null;
      FetchMovesNotifier = null;
      Query = null;
      TrayService.RemoveAllJobs();
    }
  }
}
