using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Schmogon.Model.Text;
using SmogonWP.Messages;
using SmogonWP.Services.Messaging;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.ViewModel
{
  public class MovesetViewModel : ViewModelBase
  {
    private readonly MessageReceiver<MovesetSelectedMessage> _movesetSelectedReceiver;
    private readonly MessageSender<PokemonAbilitySelectedMessage> _abilitySelectedSender;
    private readonly MessageSender<PokemonMoveSelectedMessage> _moveSelectedSender;

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

    public MovesetViewModel()
    {
      _movesetSelectedReceiver = new MessageReceiver<MovesetSelectedMessage>(onMovesetSelected, true);
      _abilitySelectedSender = new MessageSender<PokemonAbilitySelectedMessage>();
      _moveSelectedSender = new MessageSender<PokemonMoveSelectedMessage>();

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

    private void onMovesetSelected(MovesetSelectedMessage msg)
    {
      MSIVM = msg.MSIVM;
    }
  }
}
