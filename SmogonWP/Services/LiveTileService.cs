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
    public const string TileStyleKey = "tilestyle";
    public const string TileImageKey = "tileimage";

    #region main tile
    private static readonly List<string> SecretTiles = new List<string>
    {
      "arcanine",
      "beedrill",
      "braixen",
      "charizard",
      "duskull",
      "flareon",
      "furret",
      "gardevoir",
      "gastly",
      "glaceon",
      "growlithe",
      "gyarados",
      "haunter",
      "inkay",
      "jirachi",
      "jolteon",
      "koffing",
      "lilligant",
      "lopunny",
      "ludicolo",
      "lumineon",
      "m_ampharos",
      "m_blaziken",
      "m_mawile",
      "meloetta",
      "metagross",
      "milotic",
      "mismagius",
      "murkrow",
      "pancham",
      "pichu",
      "ralts",
      "rotom",
      "salamence",
      "scizor",
      "seaking",
      "slowpoke",
      "solosis",
      "squirtle",
      "suicune",
      "sylveon",
      "typhlosion",
      "tyranitar",
      "vaporeon",
      "wynaut",
      "zweilous"
    };

    private readonly IDataLoadingService _dataService;
    private readonly ISettingsService _settingsService;

    public LiveTileService(IDataLoadingService dataService, ISettingsService settingsService)
    {
      _dataService = dataService;
      _settingsService = settingsService;

      // this will transition the old tile save format to the new one
      if (_settingsService.Load("secret", false))
      {
        // if we were shuffling previously, keep on shuffling
        _settingsService.Save(TileStyleKey, 1);

        // we don't need secret anymore
        _settingsService.UnregisterSetting("secret");
      }
    }
    
    public async Task GenerateFlipTileAsync()
    {
      string name;
      Paragraph desc;

      do
      {
        var pokemon = await getRandomPokemon();

        name = pokemon.Name;
        desc = pokemon.Overview.FirstOrDefault(e => e is Paragraph) as Paragraph;

      } while (desc == null);
      
      var tileData = createTileData(name, desc.Content);

      updateTile(tileData);
    }

    public IEnumerable<Uri> GetSecretTilePaths()
    {
      return SecretTiles.Select(
        s => new Uri(Path.Combine("/Assets/Secret/", s + ".png"), 
                     UriKind.RelativeOrAbsolute));
    }

    private async Task<PokemonData> getRandomPokemon()
    {
      var rnd = new Random();

      var all = (await _dataService.FetchAllPokemonAsync()).ToList();

      var chosen = all[rnd.Next(all.Count)];

      return await _dataService.FetchPokemonDataAsync(chosen);
    }
    
    private FlipTileData createTileData(string pokemonName, string description)
    {
      var tileStyle = _settingsService.Load(TileStyleKey, 0);
      var tileImage = _settingsService.Load(TileImageKey, 0);

      var wideBackgroundImage = new Uri("/Assets/Tiles/smogon_widetile.png", UriKind.RelativeOrAbsolute);

      if (tileStyle > 0)
      {
        // if we have a chosen tile, use it
        // otherwise pick a random one
        var index = tileStyle == 2 ? tileImage : (new Random()).Next(SecretTiles.Count);

        var imagePath = SecretTiles[index];

        wideBackgroundImage = constructSecretTilePath(imagePath);
      }

      return new FlipTileData
      {
        WideBackgroundImage = wideBackgroundImage,
        WideBackBackgroundImage = new Uri(string.Empty, UriKind.Relative),
        WideBackContent = description,
        BackgroundImage = new Uri(string.Empty, UriKind.Relative),
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
