using System.IO.IsolatedStorage;

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

    public static string ConsolidateHiddenPower(string hiddenPower)
    {
      var settings = IsolatedStorageSettings.ApplicationSettings;

      var shouldConsolidate = true;

      settings.TryGetValue("xymode", out shouldConsolidate);

      if (shouldConsolidate)
      {
        return "Hidden Power";
      }
      else
      {
        return hiddenPower;
      }
    }
  }
}
