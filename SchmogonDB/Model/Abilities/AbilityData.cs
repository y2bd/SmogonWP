using System.Collections.Generic;
using SchmogonDB.Model.Text;

namespace SchmogonDB.Model.Abilities
{
  public class AbilityData : IDataItem
  {
    public AbilityData(string name, IEnumerable<ITextElement> description, IEnumerable<ITextElement> competitive)
    {
      Name = name;
      Description = description;
      Competitive = competitive;
    }

    public string Name { get; private set; }
    public IEnumerable<ITextElement> Description { get; private set; }
    public IEnumerable<ITextElement> Competitive { get; private set; }
  }
}
