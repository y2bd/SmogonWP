using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schmogon.Data
{
  public class Move : ISearchItem
  {
    public Move(string name, string pageLocation)
    {
      PageLocation = pageLocation;
      Name = name;
    }

    public string Name { get; private set; }

    public string PageLocation { get; private set; }
  }
}
