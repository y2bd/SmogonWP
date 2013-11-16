using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Schmogon.Model.Text;
using SmogonWP.Messages;
using SmogonWP.Services;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class MovesetViewModel : ViewModelBase
  {
    private readonly SimpleNavigationService _navigationService;

    private readonly MessageReceiver<ItemSelectedMessage<MovesetItemViewModel>> _movesetSelectedReceiver;
    private readonly MessageSender<ItemSelectedMessage<Ability>> _abilitySelectedSender;
    private readonly MessageSender<ItemSelectedMessage<Nature>> _natureSelectedSender;  
    private readonly MessageSender<ItemSelectedMessage<Move>> _moveSelectedSender;

    private MovesetItemViewModel _msivm;
    public MovesetItemViewModel MSIVM
    {
      get
      {
        return _msivm;
      }
      set
      {
        if (_msivm != value)
        {
          _msivm = value;
          RaisePropertyChanged(() => MSIVM);

          RaisePropertyChanged(() => Name);
          RaisePropertyChanged(() => OwnerName);
        }
      }
    }

    public string Name
    {
      get
      {
        return MSIVM.Data.Name.ToUpperInvariant();
      }
    }

    public string OwnerName
    {
      get
      {
        return MSIVM.OwnerName.ToUpperInvariant();
      }
    }

    private AbilityItemViewModel _selectedAbility;
    public AbilityItemViewModel SelectedAbility
    {
      get
      {
        return _selectedAbility;
      }
      set
      {
        if (_selectedAbility != value)
        {
          onAbilitySelected(value);

          _selectedAbility = null;
          RaisePropertyChanged(() => SelectedAbility);
        }
      }
    }

    private string _selectedNature;
    public string SelectedNature
    {
      get
      {
        return _selectedNature;
      }
      set
      {
        if (_selectedNature != value)
        {
          onNatureSelected(value);

          _selectedNature = null;
          RaisePropertyChanged(() => SelectedNature);
        }
      }
    }

    private GroupedMoveItemViewModel _selectedMove;
    public GroupedMoveItemViewModel SelectedMove
    {
      get
      {
        return _selectedMove;
      }
      set
      {
        if (_selectedMove != value)
        {
          onMoveSelected(value);

          _selectedMove = null;
          RaisePropertyChanged(() => SelectedMove);
        }
      }
    }			

    public MovesetViewModel(SimpleNavigationService navigationService)
    {
      _navigationService = navigationService;

      _movesetSelectedReceiver = new MessageReceiver<ItemSelectedMessage<MovesetItemViewModel>>(onMovesetSelected, true);

      _abilitySelectedSender = new MessageSender<ItemSelectedMessage<Ability>>();
      _natureSelectedSender = new MessageSender<ItemSelectedMessage<Nature>>();
      _moveSelectedSender = new MessageSender<ItemSelectedMessage<Move>>();

      #region design data
      if (IsInDesignMode || IsInDesignModeStatic)
      {
        var ms = new Moveset
        {
          Name = "Kickapow",
          Abilities = new List<Ability> { new Ability("Burnt Toast", "Burns toast 100% of the time", "") },
          Description = new List<ITextElement>
          {
            new Paragraph("THIS MOVESET IS REALL GOOD ECAUSE IT HITS HARD NO SQUISHY FLAMEBATE LOL")
          },
          EVSpread = new BaseStat(6, 252, 0, 0, 0, 252),
          Items = new List<Item>
          {
            new Item("Life Orb", "Kicks life in the orbs", "")
          },
          Moves = new List<IEnumerable<Move>>
          {
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "")
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "")
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", "")
            },
            new List<Move>
            {
              new Move("Chocolate Sauce", "mm dat drizzle", ""),
              new Move("Milk Drain", "gotta get dat white stuff", "")
            }
          },
          Natures = new List<Nature> { Nature.Adamant, Nature.Brave }
        };

        MSIVM = new MovesetItemViewModel("Barbasaur", ms);
      }
      #endregion design data
    }

    private void onMovesetSelected(ItemSelectedMessage<MovesetItemViewModel> msg)
    {
      MSIVM = msg.Item;
    }

    private void onAbilitySelected(AbilityItemViewModel aivm)
    {
      if (aivm == null) return;

      _abilitySelectedSender.SendMessage(new ItemSelectedMessage<Ability>(aivm.Ability));
      _navigationService.Navigate(ViewModelLocator.AbilityDataPath);
    }

    private void onNatureSelected(string nats)
    {
      if (string.IsNullOrWhiteSpace(nats)) return;

      Nature nature;

      if (Enum.TryParse(nats, true, out nature))
      {
        _natureSelectedSender.SendMessage(new ItemSelectedMessage<Nature>(nature));
        _navigationService.Navigate(ViewModelLocator.NaturePath);
      }
    }

    private void onMoveSelected(GroupedMoveItemViewModel gmivm)
    {
      if (gmivm == null) return;

      _moveSelectedSender.SendMessage(new ItemSelectedMessage<Move>(gmivm.Move));
      _navigationService.Navigate(ViewModelLocator.MoveDataPath);
    }
  }
}
