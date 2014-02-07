using System.Collections.Generic;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;

namespace SchmogonDB.Model.Teams
{
  public class TeamMember
  {
    public Pokemon.Pokemon Pokemon { get; set; }

    public IEnumerable<Move> Moves { get; set; } 

    public Ability Ability { get; set; }

    public Nature Nature { get; set; }

    public Item Item { get; set; }

    public BaseStat EVSpread { get; set; }

    public int Level { get; set; }

    public TeamMember(Pokemon.Pokemon pokemon, IEnumerable<Move> moves, Ability ability, Nature nature, Item item, BaseStat evSpread, int level)
    {
      Pokemon = pokemon;
      Moves = moves;
      Ability = ability;
      Nature = nature;
      Item = item;
      EVSpread = evSpread;
      Level = level;
    }

    public TeamMember() {}

    public override string ToString()
    {
      return Pokemon.ToString();
    }
  }
}
