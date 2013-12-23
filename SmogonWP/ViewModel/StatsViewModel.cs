using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;
using SchmogonDB.Tools;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.Utilities;
using SmogonWP.ViewModel.AppBar;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class StatsViewModel : ViewModelBase
  {
    private readonly IDataLoadingService _dataService;
    private readonly SchmogonToolset _toolset;
    private readonly TombstoneService _tombstoneService;

    private readonly MessageReceiver<MovesetCalculatedMessage> _movesetCalculatedReceiver;

    private BaseStat _baseStats;

    private int _evWarningCountdown = 0;
    private bool _clearing;

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

    #region stats props

    private int _level = 50;
    public int Level
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

          recalculateStats();
        }
      }
    }

    private int _hp;
    public int HP
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

    private int _attack;
    public int Attack
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

    private int _defense;
    public int Defense
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

    private int _specialAttack;
    public int SpecialAttack
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

    private int _specialDefense;
    public int SpecialDefense
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

    private int _speed;
    public int Speed
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

    private BaseStat _maxPossible;
    public BaseStat MaxPossible
    {
      get
      {
        return _maxPossible;
      }
      set
      {
        if (!Equals(_maxPossible, value))
        {
          _maxPossible = value;
          RaisePropertyChanged(() => MaxPossible);
        }
      }
    }

    #endregion stats props

    #region bar props

    private double _levelPct;
    public double LevelPct
    {
      get
      {
        return _levelPct;
      }
      set
      {
        if (_levelPct != value)
        {
          _levelPct = value;
          RaisePropertyChanged(() => LevelPct);
        }
      }
    }

    private double _hpPct;
    public double HPPct
    {
      get
      {
        return _hpPct;
      }
      set
      {
        if (_hpPct != value)
        {
          _hpPct = value;
          RaisePropertyChanged(() => HPPct);
        }
      }
    }

    private double _atkPct;
    public double AtkPct
    {
      get
      {
        return _atkPct;
      }
      set
      {
        if (_atkPct != value)
        {
          _atkPct = value;
          RaisePropertyChanged(() => AtkPct);
        }
      }
    }

    private double _defPct;
    public double DefPct
    {
      get
      {
        return _defPct;
      }
      set
      {
        if (_defPct != value)
        {
          _defPct = value;
          RaisePropertyChanged(() => DefPct);
        }
      }
    }

    private double _spAPct;
    public double SpAPct
    {
      get
      {
        return _spAPct;
      }
      set
      {
        if (_spAPct != value)
        {
          _spAPct = value;
          RaisePropertyChanged(() => SpAPct);
        }
      }
    }

    private double _spDPct;
    public double SpDPct
    {
      get
      {
        return _spDPct;
      }
      set
      {
        if (_spDPct != value)
        {
          _spDPct = value;
          RaisePropertyChanged(() => SpDPct);
        }
      }
    }

    private double _spePct;
    public double SpePct
    {
      get
      {
        return _spePct;
      }
      set
      {
        if (_spePct != value)
        {
          _spePct = value;
          RaisePropertyChanged(() => SpePct);
        }
      }
    }

    #endregion bar props

    #region setup props

    private ObservableCollection<PokemonItemViewModel> _allPokemon;
    public ObservableCollection<PokemonItemViewModel> AllPokemon
    {
      get
      {
        return _allPokemon;
      }
      set
      {
        if (_allPokemon != value)
        {
          _allPokemon = value;
          RaisePropertyChanged(() => AllPokemon);
        }
      }
    }

    private string _searchedPokemon;
    public string SearchedPokemon
    {
      get
      {
        return _searchedPokemon;
      }
      set
      {
        if (_searchedPokemon != value)
        {
          _searchedPokemon = value;
          RaisePropertyChanged(() => SearchedPokemon);

          // recalculateStats();
        }
      }
    }

    private ObservableCollection<string> _natureChoices;
    public ObservableCollection<string> NatureChoices
    {
      get
      {
        return _natureChoices;
      }
      set
      {
        if (_natureChoices != value)
        {
          _natureChoices = value;
          RaisePropertyChanged(() => NatureChoices);
        }
      }
    }

    private int _selectedNature;
    public int SelectedNature
    {
      get
      {
        return _selectedNature;
      }
      set
      {
        if (_selectedNature != value)
        {
          _selectedNature = value;
          RaisePropertyChanged(() => SelectedNature);

          recalculateStats();
        }
      }
    }

    #endregion setup props

    #region ev props

    private string _evHP;
    public string EVHP
    {
      get
      {
        return _evHP;
      }
      set
      {
        if (_evHP != value)
        {
          _evHP = ensureInt(value, 0, 252, _evHP);
          RaisePropertyChanged(() => EVHP);

          onEVChanged();
        }
      }
    }

    private string _evAtk;
    public string EVAtk
    {
      get
      {
        return _evAtk;
      }
      set
      {
        if (_evAtk != value)
        {
          _evAtk = ensureInt(value, 0, 252, _evAtk);
          RaisePropertyChanged(() => EVAtk);

          onEVChanged();
        }
      }
    }

    private string _evDef;
    public string EVDef
    {
      get
      {
        return _evDef;
      }
      set
      {
        if (_evDef != value)
        {
          _evDef = ensureInt(value, 0, 252, _evDef);
          RaisePropertyChanged(() => EVDef);

          onEVChanged();
        }
      }
    }

    private string _evSpA;
    public string EVSpA
    {
      get
      {
        return _evSpA;
      }
      set
      {
        if (_evSpA != value)
        {
          _evSpA = ensureInt(value, 0, 252, _evSpA);
          RaisePropertyChanged(() => EVSpA);

          onEVChanged();
        }
      }
    }

    private string _evSpD;
    public string EVSpD
    {
      get
      {
        return _evSpD;
      }
      set
      {
        if (_evSpD != value)
        {
          _evSpD = ensureInt(value, 0, 252, _evSpD);
          RaisePropertyChanged(() => EVSpD);

          onEVChanged();
        }
      }
    }

    private string _evSpe;
    public string EVSpe
    {
      get
      {
        return _evSpe;
      }
      set
      {
        if (_evSpe != value)
        {
          _evSpe = ensureInt(value, 0, 252, _evSpe);
          RaisePropertyChanged(() => EVSpe);

          onEVChanged();
        }
      }
    }

    #endregion ev props

    #region iv props

    private string _ivHP;
    public string IVHP
    {
      get
      {
        return _ivHP;
      }
      set
      {
        if (_ivHP != value)
        {
          _ivHP = ensureInt(value, 0, 31, _ivHP);
          RaisePropertyChanged(() => IVHP);

          onIVChanged();
        }
      }
    }

    private string _ivAtk;
    public string IVAtk
    {
      get
      {
        return _ivAtk;
      }
      set
      {
        if (_ivAtk != value)
        {
          _ivAtk = ensureInt(value, 0, 31, _ivAtk);
          RaisePropertyChanged(() => IVAtk);

          onIVChanged();
        }
      }
    }

    private string _ivDef;
    public string IVDef
    {
      get
      {
        return _ivDef;
      }
      set
      {
        if (_ivDef != value)
        {
          _ivDef = ensureInt(value, 0, 31, _ivDef);
          RaisePropertyChanged(() => IVDef);

          onIVChanged();
        }
      }
    }

    private string _ivSpA;
    public string IVSpA
    {
      get
      {
        return _ivSpA;
      }
      set
      {
        if (_ivSpA != value)
        {
          _ivSpA = ensureInt(value, 0, 31, _ivSpA);
          RaisePropertyChanged(() => IVSpA);

          onIVChanged();
        }
      }
    }

    private string _ivSpD;
    public string IVSpD
    {
      get
      {
        return _ivSpD;
      }
      set
      {
        if (_ivSpD != value)
        {
          _ivSpD = ensureInt(value, 0, 31, _ivSpD);
          RaisePropertyChanged(() => IVSpD);

          onIVChanged();
        }
      }
    }

    private string _ivSpe;
    public string IVSpe
    {
      get
      {
        return _ivSpe;
      }
      set
      {
        if (_ivSpe != value)
        {
          _ivSpe = ensureInt(value, 0, 31, _ivSpe);
          RaisePropertyChanged(() => IVSpe);

          onIVChanged();
        }
      }
    }

    #endregion iv props

    private RelayCommand _lostFocusCommand;
    public RelayCommand LostFocusCommand
    {
      get
      {
        return _lostFocusCommand ??
               (_lostFocusCommand = new RelayCommand(recalculateStats));
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

    public INotifyTaskCompletion FetchPokemonNotifier { get; private set; }

    public StatsViewModel(IDataLoadingService dataService, SchmogonToolset toolset, TrayService trayService, TombstoneService tombstoneService)
    {
      _dataService = dataService;
      _toolset = toolset;
      _trayService = trayService;
      _tombstoneService = tombstoneService;

      NatureChoices = new ObservableCollection<string>();

      foreach (var nature in Enum.GetValues(typeof(Nature)).Cast<Nature>())
      {
        var effect = _toolset.GetNatureEffect(nature);

        var name = Enum.GetName(typeof(Nature), nature);

        var suffix = string.Format("(+{0}, -{1})",
          StatUtils.GetShortName(effect.Increased).ToLower(),
          StatUtils.GetShortName(effect.Decreased).ToLower());

        if (name != null) NatureChoices.Add(string.Format("{0} {1}", name.ToLower(), suffix));
      }

      schedulePokemonListFetch();

      setupAppBar();

      _movesetCalculatedReceiver = new MessageReceiver<MovesetCalculatedMessage>(onMovesetCalculated, true);

      MessengerInstance.Register(this, new Action<TombstoneMessage<StatsViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<StatsViewModel>>(m => restore()));
    }

    private void setupAppBar()
    {
      MenuButtons = new ObservableCollection<MenuButtonViewModel>
      {
        new MenuButtonViewModel
        {
          Text = "clear all",
          IconUri = new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.RelativeOrAbsolute),
          Command = new RelayCommand(clearAll)
        }
      };
    }

    private void clearAll()
    {
      _clearing = true;

      SearchedPokemon = string.Empty;
      SelectedNature = 0;

      EVHP = string.Empty;
      EVAtk = string.Empty;
      EVDef = string.Empty;
      EVSpA = string.Empty;
      EVSpD = string.Empty;
      EVSpe = string.Empty;

      IVHP = string.Empty;
      IVAtk = string.Empty;
      IVDef = string.Empty;
      IVSpA = string.Empty;
      IVSpD = string.Empty;
      IVSpe = string.Empty;

      Level = 50;
      HP = 0;
      Attack = 0;
      Defense = 0;
      SpecialAttack = 0;
      SpecialDefense = 0;
      Speed = 0;

      LevelPct = 0;
      HPPct = 0;
      AtkPct = 0;
      DefPct = 0;
      SpAPct = 0;
      SpDPct = 0;
      SpePct = 0;

      _evWarningCountdown = 0;

      _clearing = false;
    }

    private void recalculateStats()
    {
      // we're messing with things that aren't fully set, don't bother
      if (_clearing) return;

      if (string.IsNullOrWhiteSpace(_searchedPokemon)) return;

      var pokemon = AllPokemon.FirstOrDefault(p => p.Name == SearchedPokemon);

      if (pokemon == null) return;

      _baseStats = pokemon.Pokemon.BaseStats;

      int evhp;
      int.TryParse(EVHP, out evhp);
      int evatk;
      int.TryParse(EVAtk, out evatk);
      int evdef;
      int.TryParse(EVDef, out evdef);
      int evspatk;
      int.TryParse(EVSpA, out evspatk);
      int evspdef;
      int.TryParse(EVSpD, out evspdef);
      int evspe;
      int.TryParse(EVSpe, out evspe);

      int ivhp;
      int.TryParse(IVHP, out ivhp);
      int ivatk;
      int.TryParse(IVAtk, out ivatk);
      int ivdef;
      int.TryParse(IVDef, out ivdef);
      int ivspatk;
      int.TryParse(IVSpA, out ivspatk);
      int ivspdef;
      int.TryParse(IVSpD, out ivspdef);
      int ivspe;
      int.TryParse(IVSpe, out ivspe);

      var effect = _toolset.GetNatureEffect((Nature)SelectedNature);

      var atkBonus = decideNatureBonus(effect, StatType.Attack);
      var defBonus = decideNatureBonus(effect, StatType.Defense);
      var spatkBonus = decideNatureBonus(effect, StatType.SpecialAttack);
      var spdefBonus = decideNatureBonus(effect, StatType.SpecialDefense);
      var speBonus = decideNatureBonus(effect, StatType.Speed);

      HP = StatCalculator.CalculateHP(Level, _baseStats.HP, evhp, ivhp);
      Attack = StatCalculator.CalculateOtherStat(Level, _baseStats.Attack, evatk, ivatk, atkBonus);
      Defense = StatCalculator.CalculateOtherStat(Level, _baseStats.Defense, evdef, ivdef, defBonus);
      SpecialAttack = StatCalculator.CalculateOtherStat(Level, _baseStats.SpecialAttack, evspatk, ivspatk, spatkBonus);
      SpecialDefense = StatCalculator.CalculateOtherStat(Level, _baseStats.SpecialDefense, evspdef, ivspdef, spdefBonus);
      Speed = StatCalculator.CalculateOtherStat(Level, _baseStats.Speed, evspe, ivspe, speBonus);

      LevelPct = ((Level / 100.0 * 100));
      HPPct = (((double)HP / StatCalculator.MaxHP * 100));
      AtkPct = (((double)Attack / StatCalculator.MaxAttack * 100));
      DefPct = (((double)Defense / StatCalculator.MaxDefense * 100));
      SpAPct = (((double)SpecialAttack / StatCalculator.MaxSpecialAttack * 100));
      SpDPct = (((double)SpecialDefense / StatCalculator.MaxSpecialDefense * 100));
      SpePct = (((double)Speed / StatCalculator.MaxSpeed * 100));
    }

    #region data fetching
    private void schedulePokemonListFetch()
    {
      FetchPokemonNotifier = NotifyTaskCompletion.Create(fetchPokemon());

      FetchPokemonNotifier.PropertyChanged += (sender, args) =>
      {
        if (FetchPokemonNotifier == null) return;

        if (FetchPokemonNotifier.IsFaulted)
        {
          throw FetchPokemonNotifier.InnerException;
        }
      };
    }

    private async Task fetchPokemon()
    {
      AllPokemon = null;

      try
      {
        var rawPokemon = await _dataService.FetchAllPokemonAsync();

        var pivms = (from pokemon in rawPokemon
                     select new PokemonItemViewModel(pokemon))
        .ToList();

        AllPokemon = new ObservableCollection<PokemonItemViewModel>(pivms);
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

    private void cleanup()
    {
      AllPokemon = null;
      FetchPokemonNotifier = null;
      SearchedPokemon = null;
      TrayService.RemoveAllJobs();
    }
    #endregion data fetching

    private string ensureInt(string value, int min, int max, string old)
    {
      if (_clearing) return value;

      if (string.IsNullOrWhiteSpace(value)) return "0";

      int val;

      return !int.TryParse(value, out val) ? old : Math.Max(min, Math.Min(max, val)).ToString(CultureInfo.InvariantCulture);
    }

    private void onEVChanged()
    {
      if (_clearing) return;

      int evhp;
      int.TryParse(EVHP, out evhp);
      int evatk;
      int.TryParse(EVAtk, out evatk);
      int evdef;
      int.TryParse(EVDef, out evdef);
      int evspatk;
      int.TryParse(EVSpA, out evspatk);
      int evspdef;
      int.TryParse(EVSpD, out evspdef);
      int evspe;
      int.TryParse(EVSpe, out evspe);

      if (evhp + evatk + evdef + evspatk + evspdef + evspe > 510)
      {
        if (--_evWarningCountdown < 0)
        {
          var toast = new ToastPrompt
          {
            Title = "A Warning",
            Message = "The sum of your Pokemon's EVs cannot be greater that 510 ingame.",
            TextOrientation = Orientation.Vertical,
            TextWrapping = TextWrapping.Wrap
          };

          toast.Show();

          _evWarningCountdown = 5;
        }
      }
      else
      {
        recalculateStats();
      }
    }

    private void onIVChanged()
    {
      if (_clearing) return;

      int ivhp;
      int.TryParse(IVHP, out ivhp);
      int ivatk;
      int.TryParse(IVAtk, out ivatk);
      int ivdef;
      int.TryParse(IVDef, out ivdef);
      int ivspatk;
      int.TryParse(IVSpA, out ivspatk);
      int ivspdef;
      int.TryParse(IVSpD, out ivspdef);
      int ivspe;
      int.TryParse(IVSpe, out ivspe);

      recalculateStats();
    }

    private void onMovesetCalculated(MovesetCalculatedMessage msg)
    {
      if (msg == null || msg.Item == null) return;

      clearAll();

      _clearing = true;

      var ms = msg.Item;

      SearchedPokemon = ms.OwnerName.ToLower();

      SelectedNature = (int)Enum.Parse(typeof(Nature), ms.Natures.First(), true);

      EVHP = ms.Data.EVSpread.HP.ToString(CultureInfo.InvariantCulture);
      EVAtk = ms.Data.EVSpread.Attack.ToString(CultureInfo.InvariantCulture);
      EVDef = ms.Data.EVSpread.Defense.ToString(CultureInfo.InvariantCulture);
      EVSpA = ms.Data.EVSpread.SpecialAttack.ToString(CultureInfo.InvariantCulture);
      EVSpD = ms.Data.EVSpread.SpecialDefense.ToString(CultureInfo.InvariantCulture);
      EVSpe = ms.Data.EVSpread.Speed.ToString(CultureInfo.InvariantCulture);

      IVHP = EVHP == "0" ? "0" : "31";
      IVAtk = EVAtk == "0" ? "0" : "31";
      IVDef = EVDef == "0" ? "0" : "31";
      IVSpA = EVSpA == "0" ? "0" : "31";
      IVSpD = EVSpD == "0" ? "0" : "31";
      IVSpe = EVSpe == "0" ? "0" : "31";

      _clearing = false;

      recalculateStats();
    }

    private static double decideNatureBonus(NatureEffect effect, StatType type)
    {
      if (effect.IsNeutral) return 1;

      if (effect.Increased == type) return 1.1;

      if (effect.Decreased == type) return 0.9;

      return 1;
    }

    private async void tombstone()
    {
      var cache = new StatsTombstone
      {
        Pokemon = SearchedPokemon,
        SelectedNature = SelectedNature,
        EV = new SerialStat
        {
          HP = EVHP,
          Atk = EVAtk,
          Def = EVDef,
          SpA = EVSpA,
          SpD = EVSpD,
          Spe = EVSpe
        },
        IV = new SerialStat
        {
          HP = IVHP,
          Atk = IVAtk,
          Def = IVDef,
          SpA = IVSpA,
          SpD = IVSpD,
          Spe = IVSpe
        },
        Level = Level
      };

      await _tombstoneService.Store("ts_stats", cache);
    }

    private async void restore()
    {
      TrayService.AddJob("statresume", "Resuming...");

      var loaded = await _tombstoneService.Load<StatsTombstone>("ts_stats");

      if (loaded != null)
      {
        SearchedPokemon = loaded.Pokemon;
        SelectedNature = loaded.SelectedNature;

        EVHP = loaded.EV.HP;
        EVAtk = loaded.EV.Atk;
        EVDef = loaded.EV.Def;
        EVSpA = loaded.EV.SpA;
        EVSpD = loaded.EV.SpD;
        EVSpe = loaded.EV.Spe;

        IVHP = loaded.IV.HP;
        IVAtk = loaded.IV.Atk;
        IVDef = loaded.IV.Def;
        IVSpA = loaded.IV.SpA;
        IVSpD = loaded.IV.SpD;
        IVSpe = loaded.IV.Spe;

        Level = loaded.Level;
        
        recalculateStats();
      }

      TrayService.RemoveJob("statresume");
    }

    public class SerialStat
    {
      public string HP { get; set; }
      public string Atk { get; set; }
      public string Def { get; set; }
      public string SpA { get; set; }
      public string SpD { get; set; }
      public string Spe { get; set; }
    }

    public class StatsTombstone
    {
      public string Pokemon { get; set; }
      public int SelectedNature { get; set; }
      public SerialStat EV { get; set; }
      public SerialStat IV { get; set; }
      public int Level { get; set; }
    }
  }
}
