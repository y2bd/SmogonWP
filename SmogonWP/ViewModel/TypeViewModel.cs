using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Schmogon;
using Schmogon.Data.Types;
using SmogonWP.Messages;
using SmogonWP.Model;
using SmogonWP.Services.Messaging;
using SmogonWP.View;
using SmogonWP.ViewModel.Items;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel
{
  public class TypeViewModel : ViewModelBase
  {
    private readonly ISchmogonClient _schmogonClient;

    private readonly MessageReceiver<MoveTypeSelectedMessage> _moveTypeSelectedMessage; 

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

    public TypeViewModel(ISchmogonClient schmogonClient)
    {
      _schmogonClient = schmogonClient;

      _moveTypeSelectedMessage = new MessageReceiver<MoveTypeSelectedMessage>(onMoveTypeSelected, true);

      setup();

      if (IsInDesignMode || IsInDesignModeStatic)
      {
        SelectedOffensiveType = 8;
        SelectedDefenseType = 4;

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
    }

    private void onOffenseTypeChange()
    {
      if (SelectedOffensiveType < 0) return;

      var type = (Type) SelectedOffensiveType;

      var effect = _schmogonClient.GetTypeOffenseEffect(type);

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

      var effect = _schmogonClient.GetTypeDefenseEffect(type);

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

    private void onMoveTypeSelected(MoveTypeSelectedMessage msg)
    {
      var type = msg.Type;

      MessengerInstance.Send(new VmToViewMessage<string, TypeView>("switchToOffense"));

      SelectedOffensiveType = (int) type;
    }
  }
}