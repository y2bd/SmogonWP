using System.Collections.Generic;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Text;

namespace SchmogonDB.Model.Pokemon
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
