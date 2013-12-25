using Microsoft.Phone.Shell;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmogonWP.Services
{
  public class LiveTileService
  {
    #region main tile
    private static readonly List<string> SecretTiles = new List<string>
    {
      "beedrill",
      "braixen",
      "charizard",
      "furret",
      "gardevoir",
      "gastly",
      "glaceon",
      "growlithe",
      "haunter",
      "inkay",
      "koffing",
      "lilligant",
      "lopunny",
      "ludicolo",
      "m_ampharos",
      "m_blaziken",
      "m_mawile",
      "meloetta",
      "milotic",
      "murkrow",
      "names",
      "pancham",
      "pichu",
      "rotom",
      "scizor",
      "solosis",
      "sylveon",
      "typhlosion",
      "wynaut",
    };

    private readonly IDataLoadingService _dataService;

    public LiveTileService(IDataLoadingService dataService)
    {
      _dataService = dataService;
    }

    public async Task GenerateFlipTileAsync(bool withSecret=false)
    {
      string name;
      Paragraph desc;

      do
      {
        var pokemon = await getRandomPokemon();

        name = pokemon.Name;
        desc = pokemon.Overview.FirstOrDefault(e => e is Paragraph) as Paragraph;

      } while (desc == null);
      
      var tileData = createTileData(name, desc.Content, withSecret);

      updateTile(tileData);
    }

    private async Task<PokemonData> getRandomPokemon()
    {
      var rnd = new Random();

      var all = (await _dataService.FetchAllPokemonAsync()).ToList();

      var chosen = all[rnd.Next(all.Count)];

      return await _dataService.FetchPokemonDataAsync(chosen);
    }
    
    private FlipTileData createTileData(string pokemonName, string description, bool withSecret)
    {
      var wideBackgroundImage = new Uri("/Assets/Tiles/smogon_widetile.png", UriKind.RelativeOrAbsolute);

      if (withSecret)
      {
        var rnd = new Random();

        var rndSecret = SecretTiles[rnd.Next(SecretTiles.Count)];

        wideBackgroundImage = constructSecretTilePath(rndSecret);
      }

      return new FlipTileData
      {
        WideBackgroundImage = wideBackgroundImage,
        WideBackBackgroundImage = new Uri(string.Empty, UriKind.Relative),
        WideBackContent = description,
        BackBackgroundImage = new Uri(string.Empty, UriKind.Relative),
        BackContent = description,
        BackTitle = pokemonName,
        Title = "SmogonWP"
      };
    }

    private void updateTile(FlipTileData tileData)
    {
      var flipTile = ShellTile.ActiveTiles.FirstOrDefault();

      if (flipTile != null)
      {
        flipTile.Update(tileData);
      }
    }

    private Uri constructSecretTilePath(string name)
    {
      var path = Path.Combine("/Assets/Secret/", name + ".png");

      return new Uri(path, UriKind.RelativeOrAbsolute);
    }

    #endregion main tiles

    #region secondary tiles
    /// <summary>
    /// Creates a secondary tile.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="navUri"></param>
    /// <param name="iconUri"></param>
    /// <returns>Returns false if the tile already exists.</returns>
    public bool CreateSecondaryTile(string title, Uri navUri, Uri iconUri)
    {
      if (tileExists(navUri)) return false;

      var tileData = createSecondaryTileData(title, iconUri);

      ShellTile.Create(navUri, tileData);

      return true;
    }

    private bool tileExists(Uri navUri)
    {
      return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.Equals(navUri)) != null;
    }

    private StandardTileData createSecondaryTileData(string title, Uri iconUri)
    {
      return new StandardTileData
      {
        Title = title ?? string.Empty,
        BackgroundImage = iconUri ?? new Uri(string.Empty, UriKind.Relative),
        BackTitle = "SmogonWP",
        BackBackgroundImage = iconUri ?? new Uri(string.Empty, UriKind.Relative)
      };
    }
    #endregion secondary tiles

  }
}
