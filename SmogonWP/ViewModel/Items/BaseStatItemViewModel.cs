using System;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Stats;

namespace SmogonWP.ViewModel.Items
{
  public class BaseStatItemViewModel : ViewModelBase
  {
    private readonly int _perStatMax;
    private readonly int _totalMax;

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
          _hp = ensureMaxes(_hp, value);
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
          _attack = ensureMaxes(_attack, value);
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
          _defense = ensureMaxes(_defense, value);
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
          _specialAttack = ensureMaxes(_specialAttack, value);
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
          _specialDefense = ensureMaxes(_specialDefense, value);
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
          _speed = ensureMaxes(_speed, value);
          RaisePropertyChanged(() => Speed);
        }
      }
    }

    public BaseStatItemViewModel(BaseStat stat, int perStatMax, int totalMax)
      : this(stat)
    {
      _perStatMax = perStatMax;
      _totalMax = totalMax;
    }

    public BaseStatItemViewModel(int perStatMax, int totalMax)
    {
      _perStatMax = perStatMax;
      _totalMax = totalMax;
    }

    public BaseStatItemViewModel(BaseStat stat)
    {
      _hp = stat.HP;
      _attack = stat.Attack;
      _defense = stat.Defense;
      _specialAttack = stat.SpecialAttack;
      _specialDefense = stat.SpecialDefense;
      _speed = stat.Speed;
    }

    public BaseStatItemViewModel()
    {}

    public BaseStat ToBaseStat()
    {
      return new BaseStat(HP, Attack, Defense, SpecialAttack, SpecialDefense, Speed);
    }

    private int ensureMaxes(int currentValue, int newValue)
    {
      // if there's a max per stat and this value is bigger, return the older value
      if (_perStatMax > 0 && newValue > _perStatMax)
      {
        OnMaxValueExceeded();
        return currentValue;
      }

      // if there's a total max and the sum is now bigger, return the older value
      if (_totalMax > 0 && (HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed) > _totalMax)
      {
        OnMaxValueExceeded();
        return currentValue;
      }

      return newValue;
    }

    public event EventHandler MaxValueExceeded;

    protected virtual void OnMaxValueExceeded()
    {
      var handler = MaxValueExceeded;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
