using System.Collections.Generic;
using Schmogon.Data.Abilities;
using Schmogon.Data.Items;
using Schmogon.Data.Moves;
using Schmogon.Data.Natures;
using Schmogon.Data.Stats;
using Schmogon.Model.Text;

namespace Schmogon.Data.Pokemon
{
  public class Moveset
  {
    public string Name { get; set; }

    public IEnumerable<Item> Items { get; set; }

    public IEnumerable<Ability> Abilities { get; set; }

    public IEnumerable<Nature> Natures { get; set; }

    /// <summary>
    /// It's a list of a list because sometimes there are multiple moves that can fill a slot
    /// </summary>
    public IEnumerable<IEnumerable<Move>> Moves { get; set; }

    public BaseStat EVSpread { get; set; }

    public ICollection<ITextElement> Description { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
