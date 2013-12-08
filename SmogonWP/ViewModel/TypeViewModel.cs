﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SchmogonDB.Tools;
using SmogonWP.Messages;
using SmogonWP.Model;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.ViewModel
{
  public class TypeViewModel : ViewModelBase
  {
    private readonly SchmogonToolset _toolset;

    private readonly MessageReceiver<ItemSelectedMessage<Type>> _moveTypeSelectedMessage;
    private readonly MessageReceiver<PokemonTypeSelectedMessage> _pokemonTypeSelectedMessage;
    
    private ObservableCollection<string> _typeChoices;
    public ObservableCollection<string> TypeChoices
    {
      get
      {
        return _typeChoices;
      }
      set
      {
        if (_typeChoices != value)
        {
          _typeChoices = value;
          RaisePropertyChanged(() => TypeChoices);
        }
      }
    }

    private ObservableCollection<string> _secondaryTypeChoices;
    public ObservableCollection<string> SecondaryTypeChoices
    {
      get
      {
        return _secondaryTypeChoices;
      }
      set
      {
        if (_secondaryTypeChoices != value)
        {
          _secondaryTypeChoices = value;
          RaisePropertyChanged(() => SecondaryTypeChoices);
        }
      }
    }			

    private int _selectedOffensiveType = -1;
    public int SelectedOffensiveType
    {
      get
      {
        return _selectedOffensiveType;
      }
      set
      {
        if (_selectedOffensiveType != value)
        {
          _selectedOffensiveType = value;
          RaisePropertyChanged(() => SelectedOffensiveType);

          onOffenseTypeChange();
        }
      }
    }			
    
    private int _selectedDefenseType = -1;
    public int SelectedDefenseType
    {
      get
      {
        return _selectedDefenseType;
      }
      set
      {
        if (_selectedDefenseType != value)
        {
          _selectedDefenseType = value;
          RaisePropertyChanged(() => SelectedDefenseType);

          onDefenseTypeChange();
        }
      }
    }

    private int _selectedSecondDefenseType = -1;
    public int SelectedSecondDefenseType
    {
      get
      {
        return _selectedSecondDefenseType;
      }
      set
      {
        if (_selectedSecondDefenseType != value)
        {
          _selectedSecondDefenseType = value;
          RaisePropertyChanged(() => SelectedSecondDefenseType);

          onDefenseTypeChange();
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
    
    private ObservableCollection<OffenseTypeGroup> _offenseTypeData;
    public ObservableCollection<OffenseTypeGroup> OffenseTypeData
    {
      get
      {
        return _offenseTypeData;
      }
      set
      {
        if (_offenseTypeData != value)
        {
          _offenseTypeData = value;
          RaisePropertyChanged(() => OffenseTypeData);
        }
      }
    }

    private ObservableCollection<DefenseTypeGroup> _defenseTypeData;
    public ObservableCollection<DefenseTypeGroup> DefenseTypeData
    {
      get
      {
        return _defenseTypeData;
      }
      set
      {
        if (_defenseTypeData != value)
        {
          _defenseTypeData = value;
          RaisePropertyChanged(() => DefenseTypeData);
        }
      }
    }

    public TypeViewModel(SchmogonToolset toolset)
    {
      _toolset = toolset;

      _moveTypeSelectedMessage = new MessageReceiver<ItemSelectedMessage<Type>>(onMoveTypeSelected, true);
      _pokemonTypeSelectedMessage = new MessageReceiver<PokemonTypeSelectedMessage>(onPokemonTypeSelected, true);

      setup();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        SelectedOffensiveType = 8;
        SelectedDefenseType = 4;
        SelectedSecondDefenseType = 6;

        onOffenseTypeChange();
        onDefenseTypeChange();
      }
    }

    private void setup()
    {
      var typeNames = Enum.GetNames(typeof(Type))
                      .Select(s => s.ToLowerInvariant())
                      .ToList();

      TypeChoices = new ObservableCollection<string>(typeNames);

      SecondaryTypeChoices = new ObservableCollection<string>(new List<string> {"none"}.Concat(typeNames));
    }

    private void onOffenseTypeChange()
    {
      if (SelectedOffensiveType < 0) return;

      var type = (Type) SelectedOffensiveType;

      var effect = _toolset.GetTypeOffenseEffect(type);

      OffenseTypeData = new ObservableCollection<OffenseTypeGroup>
      {
        new OffenseTypeGroup(
          effect.SuperEffectiveAgainst.Select(t => new TypeItemViewModel(t)),
          OffenseType.SuperEffective),
        new OffenseTypeGroup(
          effect.NotVeryEffectiveAgainst.Select(t => new TypeItemViewModel(t)),
          OffenseType.NotVeryEffective),
        new OffenseTypeGroup(
          effect.NoEffectAgainst.Select(t => new TypeItemViewModel(t)),
          OffenseType.NoEffect),
      };
    }

    private void onDefenseTypeChange()
    {
      if (SelectedDefenseType < 0) return;

      var type = (Type) SelectedDefenseType;

      if (SelectedSecondDefenseType <= 0 || SelectedSecondDefenseType - 1 == SelectedDefenseType)
      {
        var effect = _toolset.GetTypeDefenseEffect(type);

        DefenseTypeData = new ObservableCollection<DefenseTypeGroup>
        {
          new DefenseTypeGroup(
            effect.StrongDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.StrongDefense),
          new DefenseTypeGroup(
            effect.WeakDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.WeakDefense),
          new DefenseTypeGroup(
            effect.FullDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.FullDefense),
        };
      }
      else
      {
        var secondType = (Type) (SelectedSecondDefenseType - 1);

        var effect = _toolset.GetTypeDefenseEffect(type, secondType);

        DefenseTypeData = new ObservableCollection<DefenseTypeGroup>
        {
          new DefenseTypeGroup(
            effect.VeryStrongDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.VeryStrongDefense),
          new DefenseTypeGroup(
            effect.StrongDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.StrongDefense),
          new DefenseTypeGroup(
            effect.WeakDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.WeakDefense),
          new DefenseTypeGroup(
            effect.VeryWeakDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.VeryWeakDefense),
          new DefenseTypeGroup(
            effect.FullDefenseAgainst.Select(t => new TypeItemViewModel(t)),
            DefenseType.FullDefense),
        };
      }
    }
    
    private void onMoveTypeSelected(ItemSelectedMessage<Type> msg)
    {
      var type = msg.Item;

      PivotIndex = 0;

      SelectedOffensiveType = (int) type;
    }

    private void onPokemonTypeSelected(PokemonTypeSelectedMessage msg)
    {
      var type = msg.Item;

      PivotIndex = 1;

      SelectedDefenseType = (int)type;
      SelectedSecondDefenseType = 0;
    }
  }
}