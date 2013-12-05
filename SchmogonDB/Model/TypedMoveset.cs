using System.Collections.Generic;
using Schmogon.Data.Pokemon;

namespace SchmogonDB.Model
{
  public class TypedMoveset : Moveset
  {
    public new IEnumerable<IEnumerable<TypedMove>> Moves { get; set; }
  }
}
