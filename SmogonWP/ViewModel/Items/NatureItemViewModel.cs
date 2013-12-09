using System;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;

namespace SmogonWP.ViewModel.Items
{
  public class NatureItemViewModel : ViewModelBase
  {
    private string _name;
    public string Name
    {
      get
      {
        return _name.ToLowerInvariant();
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

    private string _boostStat;
    public string BoostStat
    {
      get
      {
        return _boostStat.ToLowerInvariant();
      }
      set
      {
        if (_boostStat != value)
        {
          _boostStat = value;
          RaisePropertyChanged(() => BoostStat);
        }
      }
    }

    private string _lossStat;
    public string LossStat
    {
      get
      {
        return _lossStat.ToLowerInvariant();
      }
      set
      {
        if (_lossStat != value)
        {
          _lossStat = value;
          RaisePropertyChanged(() => LossStat);
        }
      }
    }

    private int _rowIndex;
    public int RowIndex
    {
      get
      {
        return _rowIndex;
      }
      set
      {
        if (_rowIndex != value)
        {
          _rowIndex = value;
          RaisePropertyChanged(() => RowIndex);
        }
      }
    }			

    public NatureItemViewModel(NatureEffect effect)
    {
      Name = Enum.GetName(typeof(Nature), effect.Nature);
      BoostStat = StatUtils.GetName(effect.Increased);
      LossStat = StatUtils.GetName(effect.Decreased);
    }

    public NatureItemViewModel(NatureEffect effect, int rowIndex)
      : this(effect)
    {
      _rowIndex = rowIndex;
    }
  }
}
