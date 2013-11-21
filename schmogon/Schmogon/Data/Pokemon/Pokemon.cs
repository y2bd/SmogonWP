using System.Collections.Generic;
using Schmogon.Data.Abilities;
using Schmogon.Data.Stats;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon.Data.Pokemon
{
  public class Pokemon : ISearchItem
  {
    public string Name { get; private set; }

    public IEnumerable<Type> Types { get; private set; } 

    public Tier Tier { get; private set; }

    public IEnumerable<Ability> Abilities { get; private set; } 

    public BaseStat BaseStats { get; private set; }

    public string PageLocation { get; private set; }

    public Pokemon(string name, IEnumerable<Type> types, Tier tier, IEnumerable<Ability> abilities, BaseStat baseStats, string pageLocation)
    {
      Name = name;
      Types = types;
      Tier = tier;
      Abilities = abilities;
      BaseStats = baseStats;
      PageLocation = pageLocation;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
