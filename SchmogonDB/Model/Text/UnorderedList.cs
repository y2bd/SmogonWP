using System.Collections.Generic;
using System.Linq;

namespace SchmogonDB.Model.Text
{
  public class UnorderedList : ITextElement
  {
    public IEnumerable<string> Elements { get; private set; }

    public UnorderedList(IEnumerable<string> elements)
    {
      Elements = elements.Select(e => e.Trim());
    }
  }
}
