using System.Collections.Generic;
using Schmogon.Data.Abilities;
using Schmogon.Data.Moves;
using Schmogon.Data.Stats;
using Schmogon.Model.Text;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon.Data.Pokemon
{
  public class PokemonData
  {
    public string Name { get; private set; }

    public string PageLocation { get; private set; }

    public string SpritePath { get; private set; }

    public IEnumerable<Ability> Abilities { get; private set; }

    public IEnumerable<Type> Types { get; private set; }

    public Tier Tier { get; private set; }

    public BaseStat BaseStats { get; private set; }

    public IEnumerable<ITextElement> Overview { get; private set; }

    public IEnumerable<Moveset> Movesets { get; private set; } 

    public IEnumerable<ITextElement> OtherOptions { get; private set; }

    public IEnumerable<ITextElement> ChecksAndCounters { get; private set; }

    public IEnumerable<Move> Moves { get; private set; }

    public PokemonData(string name, string pageLocation, string spritePath, IEnumerable<Ability> abilities, IEnumerable<Type> types, Tier tier, BaseStat baseStats, IEnumerable<ITextElement> overview, IEnumerable<Moveset> movesets, IEnumerable<ITextElement> otherOptions, IEnumerable<ITextElement> checksAndCounters, IEnumerable<Move> moves)
    {
      Name = name;
      PageLocation = pageLocation;
      SpritePath = spritePath;
      Abilities = abilities;
      Types = types;
      Tier = tier;
      BaseStats = baseStats;
      Overview = overview;
      Movesets = movesets;
      OtherOptions = otherOptions;
      ChecksAndCounters = checksAndCounters;
      Moves = moves;
    }
  }
}
