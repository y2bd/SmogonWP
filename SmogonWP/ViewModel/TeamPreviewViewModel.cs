using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Shell;
using SchmogonDB;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;
using SchmogonDB.Tools;
using SmogonWP.Messages;
using SmogonWP.Model;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class TeamPreviewViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;
    private readonly ISchmogonDBClient _schmogonDBClient;
    private readonly SchmogonToolset _toolset;

    private readonly MessageReceiver<ItemSelectedMessage<TeamItemViewModel>> _teamSelectedReceiver;

    private TeamItemViewModel _tivm;
    public TeamItemViewModel TIVM
    {
      get
      {
        return _tivm;
      }
      set
      {
        if (_tivm != value)
        {
          _tivm = value;
          RaisePropertyChanged(() => TIVM);
        }
      }
    }

    private ObservableCollection<TeamMemberItemViewModel> _members;
    public ObservableCollection<TeamMemberItemViewModel> Members
    {
      get
      {
        return _members;
      }
      set
      {
        if (_members != value)
        {
          _members = value;
          RaisePropertyChanged(() => Members);
        }
      }
    }

    private bool _isCalculationToggleChecked = true;
    public bool IsCalculationToggleChecked
    {
      get
      {
        return _isCalculationToggleChecked;
      }
      set
      {
        if (_isCalculationToggleChecked != value)
        {
          _isCalculationToggleChecked = value;
          RaisePropertyChanged(() => IsCalculationToggleChecked);
          RaisePropertyChanged(() => CalculationToggleText);

          recalculateStats();
        }
      }
    }

    private bool _isMaxLevelToggleChecked = true;
    public bool IsMaxLevelToggleChecked
    {
      get
      {
        return _isMaxLevelToggleChecked;
      }
      set
      {
        if (_isMaxLevelToggleChecked != value)
        {
          _isMaxLevelToggleChecked = value;
          RaisePropertyChanged(() => IsMaxLevelToggleChecked);
          RaisePropertyChanged(() => MaxLevelToggleText);

          recalculateStats();
        }
      }
    }

    private StatBarItemViewModel _hp;
    public StatBarItemViewModel HP
    {
      get
      {
        return _hp;
      }
      set
      {
        if (_hp != value)
        {
          _hp = value;
          RaisePropertyChanged(() => HP);
        }
      }
    }

    private StatBarItemViewModel _attack;
    public StatBarItemViewModel Attack
    {
      get
      {
        return _attack;
      }
      set
      {
        if (_attack != value)
        {
          _attack = value;
          RaisePropertyChanged(() => Attack);
        }
      }
    }

    private StatBarItemViewModel _defense;
    public StatBarItemViewModel Defense
    {
      get
      {
        return _defense;
      }
      set
      {
        if (_defense != value)
        {
          _defense = value;
          RaisePropertyChanged(() => Defense);
        }
      }
    }

    private StatBarItemViewModel _specialAttack;
    public StatBarItemViewModel SpecialAttack
    {
      get
      {
        return _specialAttack;
      }
      set
      {
        if (_specialAttack != value)
        {
          _specialAttack = value;
          RaisePropertyChanged(() => SpecialAttack);
        }
      }
    }

    private StatBarItemViewModel _specialDefense;
    public StatBarItemViewModel SpecialDefense
    {
      get
      {
        return _specialDefense;
      }
      set
      {
        if (_specialDefense != value)
        {
          _specialDefense = value;
          RaisePropertyChanged(() => SpecialDefense);
        }
      }
    }

    private StatBarItemViewModel _speed;
    public StatBarItemViewModel Speed
    {
      get
      {
        return _speed;
      }
      set
      {
        if (_speed != value)
        {
          _speed = value;
          RaisePropertyChanged(() => Speed);
        }
      }
    }

    private StatBarItemViewModel _level;
    public StatBarItemViewModel Level
    {
      get
      {
        return _level;
      }
      set
      {
        if (_level != value)
        {
          _level = value;
          RaisePropertyChanged(() => Level);
        }
      }
    }

    private ObservableCollection<TypeCoverageGroup> _typeCoverage;
    public ObservableCollection<TypeCoverageGroup> TypeCoverage
    {
      get
      {
        return _typeCoverage;
      }
      set
      {
        if (_typeCoverage != value)
        {
          _typeCoverage = value;
          RaisePropertyChanged(() => TypeCoverage);
        }
      }
    }

    private CreatedTeamMemberItemViewModel _createdMember;
    public CreatedTeamMemberItemViewModel CreatedMember
    {
      get
      {
        return _createdMember;
      }
      set
      {
        if (_createdMember != value)
        {
          _createdMember = value;
          RaisePropertyChanged(() => CreatedMember);
        }
      }
    }			

    private RelayCommand<CancelEventArgs> _backKeyCommand;
    public RelayCommand<CancelEventArgs> BackKeyCommand
    {
      get
      {
        return _backKeyCommand ??
               (_backKeyCommand = new RelayCommand<CancelEventArgs>(onBackKeyPressed));
      }
    }

    #region ui props

    private MenuButtonViewModel _addMemberButton;
    private MenuButtonViewModel _coverageHelpButton;

    private ApplicationBarMode _appBarMode = ApplicationBarMode.Default;
    public ApplicationBarMode AppBarMode
    {
      get
      {
        return _appBarMode;
      }
      set
      {
        if (_appBarMode != value)
        {
          _appBarMode = value;
          RaisePropertyChanged(() => AppBarMode);
        }
      }
    }

    private int _pivotTab;
    public int PivotTab
    {
      get
      {
        return _pivotTab;
      }
      set
      {
        if (_pivotTab != value)
        {
          _pivotTab = value;
          RaisePropertyChanged(() => PivotTab);

          onPivotTabChanged();
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

    private bool _coverageHelpOpen;
    public bool CoverageHelpOpen
    {
      get
      {
        return _coverageHelpOpen;
      }
      set
      {
        if (_coverageHelpOpen != value)
        {
          _coverageHelpOpen = value;
          RaisePropertyChanged(() => CoverageHelpOpen);
        }
      }
    }			

    public string CalculationToggleText
    {
      get
      {
        return IsCalculationToggleChecked ? "Yes" : "No";
      }
    }

    public string MaxLevelToggleText
    {
      get
      {
        return IsMaxLevelToggleChecked ? "My Team's Max" : "100";
      }
    }

    #endregion ui props

    public TeamPreviewViewModel(SimpleNavigationService navigationService, ISchmogonDBClient schmogonDBClient, IDataLoadingService dataService, SchmogonToolset toolset, TrayService trayService)
    {
      _navigationService = navigationService;
      _schmogonDBClient = schmogonDBClient;
      _toolset = toolset;
      _trayService = trayService;

      createStatBars();

      CreatedMember = new CreatedTeamMemberItemViewModel(dataService, toolset);

      _teamSelectedReceiver = new MessageReceiver<ItemSelectedMessage<TeamItemViewModel>>(onTeamReceived, true);

      if (IsInDesignMode)
      {
        fetchDesignTeam();
      }

      setupAppBar();
    }

    private void setupAppBar()
    {
      _addMemberButton = new MenuButtonViewModel
      {
        Text = "add",
        IconUri = new Uri("/Assets/AppBar/add.png", UriKind.RelativeOrAbsolute),
        Command = new RelayCommand(createTeamMember)
      };

      _coverageHelpButton = new MenuButtonViewModel
      {
        Text = "help",
        IconUri = new Uri("/Assets/AppBar/questionmark.png", UriKind.RelativeOrAbsolute),
        Command = new RelayCommand(openCoverageHelp)
      };

      MenuButtons = new ObservableCollection<MenuButtonViewModel>();

      onPivotTabChanged();
    }

    private void onPivotTabChanged()
    {
      MenuButtons.Remove(_addMemberButton);
      MenuButtons.Remove(_coverageHelpButton);

      switch (PivotTab)
      {
        case 0:
          MenuButtons.Add(_addMemberButton);
          AppBarMode = ApplicationBarMode.Default;
          break;
        case 2:
          MenuButtons.Add(_coverageHelpButton);
          AppBarMode = ApplicationBarMode.Default;
          break;
        default:
          AppBarMode = ApplicationBarMode.Minimized;
          break;
      }
    }

    private void createStatBars()
    {
      HP = new StatBarItemViewModel(0, 1, 432);
      Attack = new StatBarItemViewModel(0, 1, 432);
      Defense = new StatBarItemViewModel(0, 1, 432);
      SpecialAttack = new StatBarItemViewModel(0, 1, 432);
      SpecialDefense = new StatBarItemViewModel(0, 1, 432);
      Speed = new StatBarItemViewModel(0, 1, 432);

      Level = new StatBarItemViewModel(0, 1, 432);
    }

    private async void fetchDesignTeam()
    {
      TIVM = new TeamItemViewModel((await _schmogonDBClient.FetchAllTeamsAsync()).First());
      await generateChildVMs();
    }

    private async void onTeamReceived(ItemSelectedMessage<TeamItemViewModel> msg)
    {
      cleanup();

      var team = msg.Item;
      if (team == null) return;

      TIVM = team;

      await generateChildVMs();
    }

    private async Task generateChildVMs()
    {
      Members = new ObservableCollection<TeamMemberItemViewModel>(TIVM.Team.TeamMembers.Select(m => new TeamMemberItemViewModel(m)));

      recalculateStats();
      recalculateTypeCoverage();

      TrayService.AddJob("fetchsprites", "Downloading sprites...");

      if (!NetUtilities.IsNetwork())
      {
        await Task.Delay(100);

        TrayService.RemoveJob("fetchsprites");

        TrayService.AddJob("fetchsprites", "Downloading sprites...");

        await Task.Delay(500);

        TrayService.RemoveJob("fetchsprites");
      }

      // we don't await it because we want it to happen on its own
      Task.WhenAll(Members.Select(fetchSprite));

      TrayService.RemoveJob("fetchsprites");
    }

    private async Task fetchSprite(TeamMemberItemViewModel member)
    {
      var spritePath = await _schmogonDBClient.FetchPokemonSpritePathAsync(member.TeamMember.Pokemon);

      try
      {
        await downloadSprite(member, spritePath);
      }
      catch (Exception)
      {
        Debugger.Break();
      }
    }

    private async Task downloadSprite(TeamMemberItemViewModel tmivm, string spritePath)
    {
      if (!NetUtilities.IsNetwork()) return;
      
      Stream s;

      using (var client = new HttpClient())
      {
        var bytes = await client.GetByteArrayAsync(spritePath);

        s = new MemoryStream(bytes);
      }

      var wbs = new WriteableBitmap(96, 96);
      wbs.SetSource(s);

      tmivm.Sprite = wbs.Resize(192, 192, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

      s.Close();
    }

    private void recalculateStats()
    {
      if (Members.Count <= 0) return;

      double teamHP =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateHP(t.Level, t.Pokemon.BaseStats.HP, t.EVSpread.HP, t.IVSpread.HP)).Sum();

      double teamAtk =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateOtherStat(t.Level, t.Pokemon.BaseStats.Attack, t.EVSpread.Attack, t.IVSpread.Attack, getNatureMultiplier(t.Nature, StatType.Attack))).Sum();

      double teamDef =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateOtherStat(t.Level, t.Pokemon.BaseStats.Defense, t.EVSpread.Defense, t.IVSpread.Defense, getNatureMultiplier(t.Nature, StatType.Defense))).Sum();

      double teamSpa =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateOtherStat(t.Level, t.Pokemon.BaseStats.SpecialAttack, t.EVSpread.SpecialAttack, t.IVSpread.SpecialAttack, getNatureMultiplier(t.Nature, StatType.SpecialAttack))).Sum();

      double teamSpd =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateOtherStat(t.Level, t.Pokemon.BaseStats.SpecialDefense, t.EVSpread.SpecialDefense, t.IVSpread.SpecialDefense, getNatureMultiplier(t.Nature, StatType.SpecialDefense))).Sum();

      double teamSpe =
        Members.Select(m => m.TeamMember)
          .Select(t => StatCalculator.CalculateOtherStat(t.Level, t.Pokemon.BaseStats.Speed, t.EVSpread.Speed, t.IVSpread.Speed, getNatureMultiplier(t.Nature, StatType.Attack))).Sum();

      var maxLevel = IsMaxLevelToggleChecked ? Members.Select(m => m.TeamMember.Level).Max() : 100;

      double topHP = StatCalculator.CalculateMaxHPAtLevel(maxLevel) * Members.Count;
      double topAtk = StatCalculator.CalculateMaxAttackAtLevel(maxLevel) * Members.Count;
      double topDef = StatCalculator.CalculateMaxDefenseAtLevel(maxLevel) * Members.Count;
      double topSpa = StatCalculator.CalculateMaxSpecialAttackAtLevel(maxLevel) * Members.Count;
      double topSpd = StatCalculator.CalculateMaxSpecialDefenseAtLevel(maxLevel) * Members.Count;
      double topSpe = StatCalculator.CalculateMaxSpeedAtLevel(maxLevel) * Members.Count;

      // this means we're showing averages
      if (IsCalculationToggleChecked)
      {
        teamHP /= Members.Count;
        teamAtk /= Members.Count;
        teamDef /= Members.Count;
        teamSpa /= Members.Count;
        teamSpd /= Members.Count;
        teamSpe /= Members.Count;

        topHP /= Members.Count;
        topAtk /= Members.Count;
        topDef /= Members.Count;
        topSpa /= Members.Count;
        topSpd /= Members.Count;
        topSpe /= Members.Count;
      }

      HP.CurrentValue = teamHP;
      HP.MaxValue = topHP;

      Attack.CurrentValue = teamAtk;
      Attack.MaxValue = topAtk;

      Defense.CurrentValue = teamDef;
      Defense.MaxValue = topDef;

      SpecialAttack.CurrentValue = teamSpa;
      SpecialAttack.MaxValue = topSpa;

      SpecialDefense.CurrentValue = teamSpd;
      SpecialDefense.MaxValue = topSpd;

      Speed.CurrentValue = teamSpe;
      Speed.MaxValue = topSpe;

      Level.CurrentValue = maxLevel;
      Level.MaxValue = 100;
    }

    private void recalculateTypeCoverage()
    {
      var offense = _toolset.GetTeamOffenseWeaknesses(TIVM.Team);
      var defense = _toolset.GetTeamDefenseWeaknesses(TIVM.Team);

      TypeCoverage = new ObservableCollection<TypeCoverageGroup>
      {
        new TypeCoverageGroup(offense.Select(o => new TypeItemViewModel(o)), CoverageType.OffenseCoverage),
        new TypeCoverageGroup(defense.Select(o => new TypeItemViewModel(o)), CoverageType.DefenseCoverage),
      };
    }

    private double getNatureMultiplier(Nature nature, StatType statType)
    {
      return _toolset.NatureBoostsStat(nature, statType)
        ? 1.1
        : (_toolset.NatureLowersStat(nature, statType) ? 0.9 : 1.0);
    }

    private async void createTeamMember()
    {
      
    }

    private void openCoverageHelp()
    {
      CoverageHelpOpen = true;
      AppBarMode = ApplicationBarMode.Minimized;
    }

    private void onBackKeyPressed(CancelEventArgs e)
    {
      if (CoverageHelpOpen)
      {
        CoverageHelpOpen = false;
        AppBarMode = ApplicationBarMode.Default;

        e.Cancel = true;
      }
    }

    private void cleanup()
    {
      TIVM = null;
      Members = null;
    }
  }
}
