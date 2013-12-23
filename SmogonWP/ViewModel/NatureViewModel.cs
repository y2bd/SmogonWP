using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;
using SchmogonDB.Tools;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class NatureViewModel : ViewModelBase
  {
    private readonly SchmogonToolset _toolset;
    private readonly TombstoneService _tombstoneService;

    private readonly MessageReceiver<ItemSelectedMessage<Nature>> _natureSelectedReceiver; 

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

    private NatureItemViewModel _natureHad;
    public NatureItemViewModel NatureHad
    {
      get
      {
        return _natureHad;
      }
      set
      {
        if (_natureHad != value)
        {
          _natureHad = value;
          RaisePropertyChanged(() => NatureHad);
        }
      }
    }			

    private ObservableCollection<string> _statChoices;
    public ObservableCollection<string> StatChoices
    {
      get
      {
        return _statChoices;
      }
      set
      {
        if (value != _statChoices)
        {
          _statChoices = value;
          RaisePropertyChanged(() => StatChoices);
        }
      }
    }

    private ObservableCollection<NatureItemViewModel> _naturesWanted;
    public ObservableCollection<NatureItemViewModel> NaturesWanted
    {
      get
      {
        return _naturesWanted;
      }
      set
      {
        if (_naturesWanted != value)
        {
          _naturesWanted = value;
          RaisePropertyChanged(() => NaturesWanted);
        }
      }
    }			

    private int _selectedNature = -1;
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

          onSelectedNatureChange();
        }
      }
    }

    private int _selectedBoostStat = -1;
    public int SelectedBoostStat
    {
      get
      {
        return _selectedBoostStat;
      }
      set
      {
        if (_selectedBoostStat != value)
        {
          _selectedBoostStat = value;
          RaisePropertyChanged(() => SelectedBoostStat);

          onSelectedStatsChange();
        }
      }
    }

    private int _selectedLossStat = -1;
    public int SelectedLossStat
    {
      get
      {
        return _selectedLossStat;
      }
      set
      {
        if (_selectedLossStat != value)
        {
          _selectedLossStat = value;
          RaisePropertyChanged(() => SelectedLossStat);

          onSelectedStatsChange();
        }
      }
    }

    private int _pivotIndex;
    public int PivotIndex
    {
      get
      {
        return _pivotIndex;
      }
      set
      {
        if (_pivotIndex != value)
        {
          _pivotIndex = value;
          RaisePropertyChanged(() => PivotIndex);
        }
      }
    }

    public NatureViewModel(SchmogonToolset toolset, TombstoneService tombstoneService)
    {
      _toolset = toolset;
      _tombstoneService = tombstoneService;
      _natureSelectedReceiver = new MessageReceiver<ItemSelectedMessage<Nature>>(onNatureSelected, true);

      setup();

      MessengerInstance.Register(this, new Action<TombstoneMessage<AbilityDataViewModel>>(m => tombstone()));
      MessengerInstance.Register(this, new Action<RestoreMessage<AbilityDataViewModel>>(m => restore()));
    }

    private void setup()
    {
      _natureChoices = new ObservableCollection<string>();

      foreach (var nature in Enum.GetNames(typeof(Nature)))
      {
        _natureChoices.Add(nature.ToLowerInvariant());
      }

      _statChoices = new ObservableCollection<string>{"i don't care"};

      foreach (var stat in Enum.GetValues(typeof (StatType)).Cast<StatType>())
      {
        // NO HP PLS
        if (stat == StatType.HP) continue;

        _statChoices.Add(StatUtils.GetName(stat).ToLowerInvariant());
      }
    }

    private void onSelectedNatureChange()
    {
      var nature = (Nature) SelectedNature;

      NatureHad = new NatureItemViewModel(_toolset.GetNatureEffect(nature));
    }

    private void onSelectedStatsChange()
    {
      // if either of them hasn't been chosen then forget about it
      if (SelectedBoostStat < 0 || SelectedLossStat < 0) return;

      // the zero option is the wildcard option
      var boost = SelectedBoostStat == 0 ? null : ((StatType?) (SelectedBoostStat - 1));
      var loss = SelectedLossStat == 0 ? null : ((StatType?)(SelectedLossStat - 1));

      if (boost == null && loss == null)
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _toolset.GetAllNatureEffects()
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else if (boost == null)
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _toolset.GetNatureEffectsWhereDecreased((StatType)loss)
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else if (loss == null)
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _toolset.GetNatureEffectsWhereIncreased((StatType)boost)
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _toolset.GetNatureEffectsWhere((StatType)boost, (StatType)loss)
          .Select(n => new NatureItemViewModel(n))
        );
      }
    }

    private void onNatureSelected(ItemSelectedMessage<Nature> msg)
    {
      SelectedNature = (int) msg.Item;
      PivotIndex = 1;
    }

    private async void tombstone()
    {
      var cache = new NatureTombstone
      {
        PivotIndex = PivotIndex,
        SelectedNature = SelectedNature,
        SelectedBoost = SelectedBoostStat,
        SelectedLower = SelectedLossStat
      };

      await _tombstoneService.Store("ts_nature", cache);
    }

    private async void restore()
    {
      var loaded = await _tombstoneService.Load<NatureTombstone>("ts_nature");

      if (loaded != null)
      {
        SelectedNature = loaded.SelectedNature;
        SelectedBoostStat = loaded.SelectedBoost;
        SelectedLossStat = loaded.SelectedLower;
        PivotIndex = loaded.PivotIndex;
      }
    }

    public class NatureTombstone
    {
      public int SelectedNature { get; set; }
      public int SelectedBoost { get; set; }
      public int SelectedLower { get; set; }
      public int PivotIndex { get; set; }
    }
  }
}