using System.Collections.Generic;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Types;

namespace SchmogonDB.Model.Pokemon
{
  public class Pokemon : ISearchItem
  {
    public string Name { get; set; }

    public IEnumerable<Type> Types { get; set; } 

    public Tier Tier { get; set; }

    public IEnumerable<Ability> Abilities { get; set; } 

    public BaseStat BaseStats { get; set; }

    public string PageLocation { get; set; }

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
