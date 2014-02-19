using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Teams;
using SchmogonDB.Tools;
using SmogonWP.Services;

namespace SmogonWP.ViewModel.Items
{
  public class CreatedTeamMemberItemViewModel : ViewModelBase
  {
    private readonly IDataLoadingService _dataService;
    private readonly SchmogonToolset _toolset;

    #region ui props

    private Pokemon _pokemon;
    public Pokemon Pokemon
    {
      get
      {
        return _pokemon;
      }
      set
      {
        if (_pokemon != value)
        {
          _pokemon = value;
          RaisePropertyChanged(() => Pokemon);

          readPokemonData();
        }
      }
    }

    private string _pokemonText;
    public string PokemonText
    {
      get
      {
        return _pokemonText;
      }
      set
      {
        if (_pokemonText != value)
        {
          _pokemonText = value;
          RaisePropertyChanged(() => PokemonText);
        }
      }
    }			

    private Moveset _moveset;
    public Moveset Moveset
    {
      get
      {
        return _moveset;
      }
      set
      {
        if (_moveset != value)
        {
          _moveset = value;
          RaisePropertyChanged(() => Moveset);

          onMovesetSelected();
        }
      }
    }			
    
    private Move _move1;
    public Move Move1
    {
      get
      {
        return _move1;
      }
      set
      {
        if (_move1 != value)
        {
          _move1 = value;
          RaisePropertyChanged(() => Move1);
        }
      }
    }

    private Move _move2;
    public Move Move2
    {
      get
      {
        return _move2;
      }
      set
      {
        if (_move2 != value)
        {
          _move2 = value;
          RaisePropertyChanged(() => Move2);
        }
      }
    }

    private Move _move3;
    public Move Move3
    {
      get
      {
        return _move3;
      }
      set
      {
        if (_move3 != value)
        {
          _move3 = value;
          RaisePropertyChanged(() => Move3);
        }
      }
    }

    private Move _move4;
    public Move Move4
    {
      get
      {
        return _move4;
      }
      set
      {
        if (_move4 != value)
        {
          _move4 = value;
          RaisePropertyChanged(() => Move4);
        }
      }
    }

    private Ability _ability;
    public Ability Ability
    {
      get
      {
        return _ability;
      }
      set
      {
        if (_ability != value)
        {
          _ability = value;
          RaisePropertyChanged(() => Ability);
        }
      }
    }

    private int _nature;
    public int Nature
    {
      get
      {
        return _nature;
      }
      set
      {
        if (_nature != value)
        {
          _nature = value;
          RaisePropertyChanged(() => Nature);
        }
      }
    }

    private Item _item;
    public Item Item
    {
      get
      {
        return _item;
      }
      set
      {
        if (_item != value)
        {
          _item = value;
          RaisePropertyChanged(() => Item);
        }
      }
    }

    private int _level;
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
          _level = Math.Max(Math.Min(100, value), 1);
          RaisePropertyChanged(() => Level);
        }
      }
    }

    private BaseStatItemViewModel _evSpread;
    public BaseStatItemViewModel EVSpread
    {
      get
      {
        return _evSpread;
      }
      set
      {
        if (_evSpread != value)
        {
          _evSpread = value;
          RaisePropertyChanged(() => EVSpread);
        }
      }
    }

    private BaseStatItemViewModel _ivSpread;
    public BaseStatItemViewModel IVSpread
    {
      get
      {
        return _ivSpread;
      }
      set
      {
        if (_ivSpread != value)
        {
          _ivSpread = value;
          RaisePropertyChanged(() => IVSpread);
        }
      }
    }

    private bool _pivotLocked;
    public bool PivotLocked
    {
      get
      {
        return _pivotLocked;
      }
      set
      {
        if (_pivotLocked != value)
        {
          _pivotLocked = value;
          RaisePropertyChanged(() => PivotLocked);
        }
      }
    }			

    #endregion ui props

    private List<Pokemon> _allPokemon;
    public List<Pokemon> AllPokemon
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

    private List<Moveset> _allMovesets;
    public List<Moveset> AllMovesets
    {
      get
      {
        return _allMovesets;
      }
      set
      {
        if (_allMovesets != value)
        {
          _allMovesets = value;
          RaisePropertyChanged(() => AllMovesets);
        }
      }
    }			

    private List<Move> _allMoves;
    public List<Move> AllMoves
    {
      get
      {
        return _allMoves;
      }
      set
      {
        if (_allMoves != value)
        {
          _allMoves = value;
          RaisePropertyChanged(() => AllMoves);
        }
      }
    }

    private List<Ability> _allAbilities;
    public List<Ability> AllAbilities
    {
      get
      {
        return _allAbilities;
      }
      set
      {
        if (_allAbilities != value)
        {
          _allAbilities = value;
          RaisePropertyChanged(() => AllAbilities);
        }
      }
    }

    private List<Item> _allItems;
    public List<Item> AllItems
    {
      get
      {
        return _allItems;
      }
      set
      {
        if (_allItems != value)
        {
          _allItems = value;
          RaisePropertyChanged(() => AllItems);
        }
      }
    }

    private List<string> _allNatures;
    public List<string> AllNatures
    {
      get
      {
        return _allNatures;
      }
      set
      {
        if (_allNatures != value)
        {
          _allNatures = value;
          RaisePropertyChanged(() => AllNatures);
        }
      }
    }			

    public CreatedTeamMemberItemViewModel(IDataLoadingService dataService, SchmogonToolset toolset)
    {
      _dataService = dataService;
      _toolset = toolset;

      EVSpread = new BaseStatItemViewModel(252, 255*2);
      IVSpread = new BaseStatItemViewModel(new BaseStat(31, 31, 31, 31, 31, 31), 31, 31*6);

      createChoiceLists();
    }

    private async void createChoiceLists()
    {
      // this should take no time because they're already cached
      // right?
      AllPokemon = (await _dataService.FetchAllPokemonAsync()).ToList();
      AllItems = (await _dataService.FetchAllItemsAsync()).ToList();

      AllNatures = new List<string>();
      foreach (var nature in Enum.GetValues(typeof(Nature)).Cast<Nature>())
      {
        var effect = _toolset.GetNatureEffect(nature);

        var name = Enum.GetName(typeof(Nature), nature);

        var suffix = string.Format("(+{0}, -{1})",
          StatUtils.GetShortName(effect.Increased).ToLower(),
          StatUtils.GetShortName(effect.Decreased).ToLower());

        if (name != null) AllNatures.Add(string.Format("{0}\t{1}", name.ToLower(), suffix));
      }

      AllMoves = new List<Move>();
      AllItems = new List<Item>();
      AllAbilities = new List<Ability>();
      AllMovesets = new List<Moveset>();

      if (IsInDesignMode)
      {
        Pokemon = AllPokemon.Find(p => p.Name == "Barbasaur");
      }
    }

    private async void readPokemonData()
    {
      if (Pokemon == null) return;

      PivotLocked = true;

      var pdata = await _dataService.FetchPokemonDataAsync(Pokemon);

      AllMoves = pdata.Moves.ToList();
      AllAbilities = pdata.Abilities.ToList();
      AllMovesets = pdata.Movesets.ToList();

      Moveset = AllMovesets.First();
    }

    private void onMovesetSelected()
    {
      if (Moveset == null) return;

      Ability = Moveset.Abilities.First();
      Item = Moveset.Items.First();
      Nature = (int) Moveset.Natures.First();

      EVSpread = new BaseStatItemViewModel(Moveset.EVSpread, 252, 255*2);

      var moves = Moveset.Moves.ToList();

      Move1 = moves[0].First();
      Move2 = moves[1].First();
      Move3 = moves[2].First();
      Move4 = moves[3].First();
    }

    public TeamMember ToTeamMember()
    {
      return new TeamMember
      {
        Ability = Ability,
        EVSpread = EVSpread.ToBaseStat(),
        Item = Item,
        IVSpread = IVSpread.ToBaseStat(),
        Level = Level,
        Moves = new List<Move> {Move1, Move2, Move3, Move4},
        Nature = (Nature) Nature,
        Pokemon = Pokemon
      };
    }
  }
}
