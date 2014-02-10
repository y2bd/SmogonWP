using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using SchmogonDB;
using SmogonWP.Services;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class TeamBuilderViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonDBClient _schmogonDBClient;

    private ObservableCollection<TeamItemViewModel> _teams;
    public ObservableCollection<TeamItemViewModel> Teams
    {
      get
      {
        return _teams;
      }
      set
      {
        if (_teams != value)
        {
          _teams = value;
          RaisePropertyChanged(() => Teams);
        }
      }
    }			

    private MenuButtonViewModel _createTeamButton;
    private MenuButtonViewModel _confirmCreationButton;

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

    private ObservableCollection<MenuButtonViewModel> _menuButtons;
    public ObservableCollection<MenuButtonViewModel> MenuButtons
    {
      get
      {
        return _menuButtons;
      }
      set
      {
        if (_menuButtons != value)
        {
          _menuButtons = value;
          RaisePropertyChanged(() => MenuButtons);
        }
      }
    }

    private TeamAddState _teamAddState = TeamAddState.NotAdding;
    public TeamAddState TeamAddState
    {
      get
      {
        return _teamAddState;
      }
      set
      {
        if (_teamAddState != value)
        {
          _teamAddState = value;
          RaisePropertyChanged(() => TeamAddState);

          viewStateChanged();
        }
      }
    }

    private RelayCommand<CancelEventArgs> _backKeyCommand;
    public RelayCommand<CancelEventArgs> BackKeyCommand
    {
      get
      {
        return _backKeyCommand ?? (_backKeyCommand = new RelayCommand<CancelEventArgs>(onBackKeyPressed));
      }
    }

    public INotifyTaskCompletion FetchTeamsNotifier { get; private set; }

    public TeamBuilderViewModel(SimpleNavigationService navigationService, ISchmogonDBClient schmogonDBClient, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonDBClient = schmogonDBClient;
      _trayService = trayService;

      setupNavigation();
      scheduleTeamFetch();
    }

    private void setupNavigation()
    {
      _createTeamButton = new MenuButtonViewModel
      {
        Command = new RelayCommand(createNewTeam),
        IconUri = new Uri("/Assets/AppBar/add.png", UriKind.RelativeOrAbsolute),
        Text = "create team"
      };

      _confirmCreationButton = new MenuButtonViewModel
      {
        Command = new RelayCommand(confirmTeamCreation),
        IconUri = new Uri("/Assets/AppBar/check.png", UriKind.RelativeOrAbsolute),
        Text = "confirm"
      };

      _menuButtons = new ObservableCollection<MenuButtonViewModel> {_createTeamButton};
    }

    private void scheduleTeamFetch()
    {
      FetchTeamsNotifier = NotifyTaskCompletion.Create(fetchTeams());

      FetchTeamsNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchTeamsNotifier == null) return;

        if (FetchTeamsNotifier.IsFaulted) throw FetchTeamsNotifier.InnerException;
      };
    }

    private async Task fetchTeams()
    {
      Teams = null;

      try
      {
        Teams = new ObservableCollection<TeamItemViewModel>((await _schmogonDBClient.FetchAllTeamsAsync()).Select(t => new TeamItemViewModel(t)));
      }
      catch (Exception)
      {
        MessageBox.Show(
          "Your pokemon data may be corrupted. Please restart the app and try again. If this is happening a lot, please contact the developer.",
          "Oh no!", MessageBoxButton.OK);

        Debugger.Break();

        cleanup();
      }
    }
    
    private void createNewTeam()
    {
      TeamAddState = TeamAddState.Adding;
    }

    private void confirmTeamCreation()
    {
      TeamAddState = TeamAddState.NotAdding;
    }

    private void onBackKeyPressed(CancelEventArgs args)
    {
      if (TeamAddState == TeamAddState.Adding)
      {
        TeamAddState = TeamAddState.NotAdding;
        args.Cancel = true;
      }
    }

    private void viewStateChanged()
    {
      if (TeamAddState == TeamAddState.Adding)
      {
        MenuButtons.Remove(_createTeamButton);
        MenuButtons.Remove(_confirmCreationButton);

        MenuButtons.Add(_confirmCreationButton);
      }
      else
      {
        MenuButtons.Remove(_createTeamButton);
        MenuButtons.Remove(_confirmCreationButton);

        MenuButtons.Add(_createTeamButton);
      }
    }

    private void cleanup()
    {
      Teams = null;
      FetchTeamsNotifier = null;
      TrayService.RemoveAllJobs();
    }
  }

  public enum TeamAddState
  {
    NotAdding,
    Adding
  }
}
