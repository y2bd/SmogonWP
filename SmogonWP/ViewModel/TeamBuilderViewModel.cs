using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Nito.AsyncEx;
using SchmogonDB;
using SchmogonDB.Model.Teams;
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

    private IEnumerable<string> _teamTypes;
    public IEnumerable<string> TeamTypes
    {
      get
      {
        return _teamTypes ?? (_teamTypes = Enum.GetNames(typeof (TeamType)).Select(s => s.ToLower()));
      }
    }

    private string _enteredTeamName;
    public string EnteredTeamName
    {
      get
      {
        return _enteredTeamName;
      }
      set
      {
        if (_enteredTeamName != value)
        {
          _enteredTeamName = value;
          RaisePropertyChanged(() => EnteredTeamName);
        }
      }
    }

    private int _selectedTeamType;
    public int SelectedTeamType
    {
      get
      {
        return _selectedTeamType;
      }
      set
      {
        if (_selectedTeamType != value)
        {
          _selectedTeamType = value;
          RaisePropertyChanged(() => SelectedTeamType);
        }
      }
    }

    private RelayCommand<TeamItemViewModel> _deleteTeamCommand;
    public RelayCommand<TeamItemViewModel> DeleteTeamCommand
    {
      get
      {
        return _deleteTeamCommand ?? (_deleteTeamCommand = new RelayCommand<TeamItemViewModel>(deleteTeam));
      }
    }

    #region ui props

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

          addStateChanged();
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

    #endregion ui props
    
    public TeamBuilderViewModel(SimpleNavigationService navigationService, ISchmogonDBClient schmogonDBClient, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonDBClient = schmogonDBClient;
      _trayService = trayService;

      setupNavigation();
      fetchTeams();
    }

    private void setupNavigation()
    {
      _createTeamButton = new MenuButtonViewModel
      {
        Command = new RelayCommand(openCreateTeamPanel),
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
    
    private async void fetchTeams()
    {
      Teams = null;

      try
      {
        var fetched = await _schmogonDBClient.FetchAllTeamsAsync();
        Teams = new ObservableCollection<TeamItemViewModel>(fetched.Reverse().Select(t => new TeamItemViewModel(t)));
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

    private async void createTeam(string name, TeamType type)
    {
      var team = await _schmogonDBClient.CreateNewTeamAsync(name, type);

      Teams.Insert(0, new TeamItemViewModel(team));
    }

    private async void deleteTeam(TeamItemViewModel tivm)
    {
      var cmb = new CustomMessageBox
      {
        Caption = "Delete this team?",
        Message = "Are you sure you want to delete this team? You can't bring it back after deleting it.",
        LeftButtonContent = "delete",
        RightButtonContent = "cancel",
      };
      
      if (await cmb.ShowAsync() != CustomMessageBoxResult.LeftButton) return;

      var could = Teams.Remove(tivm);

      if (could) await _schmogonDBClient.DeleteTeamAsync(tivm.Team);
    }
    
    #region ui

    private void openCreateTeamPanel()
    {
      EnteredTeamName = string.Empty;
      SelectedTeamType = 0;

      TeamAddState = TeamAddState.Adding;
    }

    private void confirmTeamCreation()
    {
      TeamAddState = TeamAddState.NotAdding;

      createTeam(EnteredTeamName, (TeamType) SelectedTeamType);
    }

    private void onBackKeyPressed(CancelEventArgs args)
    {
      if (TeamAddState == TeamAddState.Adding)
      {
        TeamAddState = TeamAddState.NotAdding;
        args.Cancel = true;
      }
    }

    private void addStateChanged()
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

    #endregion ui

    private void cleanup()
    {
      Teams = null;
      TrayService.RemoveAllJobs();
    }
  }

  public enum TeamAddState
  {
    NotAdding,
    Adding
  }
}
