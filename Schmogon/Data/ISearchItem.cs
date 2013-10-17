using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schmogon.Data
{
  public interface ISearchItem
  {
    string Name { get; }

    string PageLocation { get; }
  }
}
