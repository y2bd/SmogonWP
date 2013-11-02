using System.Collections.Generic;
using System.Linq;

namespace Schmogon.Model.Text
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
