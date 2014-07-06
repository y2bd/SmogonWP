using System.Collections.Generic;
using SchmogonDB.Model.Pokemon;

namespace SmogonWP.Services
{
  public static class SpritePathConstructor
  {
    private const string BwBasePath = "http://www.smogon.com/dex/media/sprites/bw/";

    private static readonly IDictionary<string, string> NameReplacementTable =
      new Dictionary<string, string>
      {
        {"basculin-b", "basculin"},
        {"darmanitan-z", "darmanitan"},
        {"deoxys-a", "deoxys-attack"},
        {"deoxys-d", "deoxys-defense"},
        {"deoxys-s", "deoxys-speed"},
        {"farfetch'd", "farfetched"},
        {"giratina-o", "giratina-origin"},
        {"kyurem-b", "kyurem-black"},
        {"kyurem-w", "kyurem-white"},
        {"landorus-t", "landorus-therian"},
        {"meloetta-p", "meloetta-pirouette"},
        {"mime jr.", "mimejr"},
        {"mr. mime", "mrmime"},
        {"nidoran-f", "nidoranf"},
        {"nidoran-m", "nidoran-m"},
        {"rotom-f", "rotom-fan"},
        {"rotom-s", "rotom-frost"},
        {"rotom-h", "rotom-heat"},
        {"rotom-c", "rotom-mow"},
        {"rotom-w", "rotom-wash"},
        {"shaymin-s", "shaymin-sky"},
        {"thundurus-t", "thundurus-therian"},
        {"tornadus-t", "tornadus-therian"},
        {"wormadam-s", "wormadam-sandy"},
        {"wormadam-t", "wormadam-trash"},
      };

    public static string ConstructSpritePath(PokemonData pokemonData)
    {
      return ConstructSpritePath(pokemonData.Name);
    }

    public static string ConstructSpritePath(string pokemonName)
    {
      pokemonName = pokemonName.ToLower();

      pokemonName = ReplaceNameIfNecessary(pokemonName);

      var constructSpritePath = BwBasePath + pokemonName + ".gif";
      return constructSpritePath;
    }

    public static string ReplaceNameIfNecessary(string pokemonName)
    {
      if (NameReplacementTable.ContainsKey(pokemonName))
      {
        pokemonName = NameReplacementTable[pokemonName];
      }

      return pokemonName;
    }
  }
}
