using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Schmogon;
using Schmogon.Data.Natures;
using Schmogon.Data.Stats;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class NatureViewModel : ViewModelBase
  {
    private readonly ISchmogonClient _schmogonClient;

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

    public NatureViewModel(ISchmogonClient schmogonClient)
    {
      _schmogonClient = schmogonClient;

      setup();
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

        _statChoices.Add(StatUtils.GetStatName(stat).ToLowerInvariant());
      }
    }

    private void onSelectedNatureChange()
    {
      var nature = (Nature) SelectedNature;

      NatureHad = new NatureItemViewModel(_schmogonClient.GetNatureEffect(nature));
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
          _schmogonClient.GetAllNatureEffects()
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else if (boost == null)
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _schmogonClient.GetNatureEffectsWhereDecreased((StatType)loss)
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else if (loss == null)
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _schmogonClient.GetNatureEffectsWhereIncreased((StatType)boost)
          .Select(n => new NatureItemViewModel(n))
        );
      }
      else
      {
        NaturesWanted = new ObservableCollection<NatureItemViewModel>(
          _schmogonClient.GetNatureEffectsWhere((StatType)boost, (StatType)loss)
          .Select(n => new NatureItemViewModel(n))
        );
      }
    }
  }
}