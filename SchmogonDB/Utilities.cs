using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchmogonDB
{
  internal static class Utilities
  {
    public const string PokemonBasePath = "/bw/pokemon/";
    public const string AbilityBasePath = "/bw/abilities/";
    public const string MoveBasePath = "/bw/moves/";
    public const string ItemBasePath = "/bw/items/";

    public static string ConstructSmogonLink(string name, string basePath)
    {
      name = name.ToLowerInvariant().Trim();

      name = name.Replace(' ', '_');
      name = name.Replace("\'", "");

      return basePath + name;
    }
  }
}
