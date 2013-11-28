using System.ComponentModel;
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class MoveSearchViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly DataLoadingService _dataService;

    private readonly MessageSender<ItemSearchedMessage<Move>> _moveSearchSender;

    private bool _failedOnce;

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

    private string _typeName;
    public string TypeName
    {
      get
      {
        return _typeName;
      }
      set
      {
        if (_typeName != value)
        {
          _typeName = value;
          RaisePropertyChanged(() => TypeName);
        }
      }
    }

    private SolidColorBrush _typeBrush;
    public SolidColorBrush TypeBrush
    {
      get
      {
        return _typeBrush;
      }
      set
      {
        if (_typeBrush != value)
        {
          _typeBrush = value;
          RaisePropertyChanged(() => TypeBrush);
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

    public MoveSearchViewModel(SimpleNavigationService navigationService, DataLoadingService dataService, TrayService trayService)
    {
      _navigationService = navigationService;
      _dataService = dataService;
      _trayService = trayService;

      _moveSearchSender = new MessageSender<ItemSearchedMessage<Move>>();

      Filters = new ObservableCollection<string> {"none"};

      foreach (var type in Enum.GetNames(typeof (Type)).Where(s => !s.Equals("Fairy")))
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
      
      FilteredMoves = new ObservableCollection<MoveItemViewModel>(
        _moves.Where(
          m => m.Name.ToLower().Contains(Query.ToLower().Trim())
        ).OrderBy(m => m.Name)
      );
    }

    private void onFilterChanged()
    {
      var filter = SelectedFilter - 1;

      var name = Enum.GetName(typeof (Type), (Type) filter);
      if (name != null)
        TypeName = filter == -1 ? string.Empty : name.ToUpper();
      TypeBrush = filter == -1 ? null : new SolidColorBrush(TypeItemViewModel.TypeColors[(Type) filter]);

      Query = string.Empty;

      scheduleMoveListFetch();
    }

    private void onMoveSelected(MoveItemViewModel mivm)
    {
      _moveSearchSender.SendMessage(new ItemSearchedMessage<Move>(mivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
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

      var filter = SelectedFilter - 1;
      
      try
      {
        var rawMoves = filter == -1
          ? await _dataService.FetchAllMovesAsync()
          : await _dataService.FetchAllMovesOfTypeAsync((Type) filter);

        _moves = (from rawMove in rawMoves
                  select new MoveItemViewModel(rawMove))
        .ToList();

        FilteredMoves = new ObservableCollection<MoveItemViewModel>(_moves);

        LoadFailed = false;
      }
      catch (Exception e)
      {
        if (!NetUtilities.IsNetwork())
        {
          MessageBox.Show(
          "Downloading move data requires an internet connection. Please get one of those and try again later.",
          "No internet!", MessageBoxButton.OK);
        }
        else
        {
          MessageBox.Show(
          "I'm sorry, but we couldn't load the move data. Perhaps your internet is down?\n\nIf this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);
        }

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
