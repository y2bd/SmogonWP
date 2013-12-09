using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Text;

namespace SmogonWP.Services
{
  public class LiveTileService
  {
    private readonly IDataLoadingService _dataService;

    public LiveTileService(IDataLoadingService dataService)
    {
      _dataService = dataService;
    }

    public async Task GenerateFlipTileAsync()
    {
      var pokemon = await getRandomPokemon();

      var name = pokemon.Name;
      var desc = pokemon.Overview.First(e => e is Paragraph) as Paragraph;

      if (desc == null)
      {
        return;
      }
      
      var tileData = createTileData(name, desc.Content);

      updateTile(tileData);
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
      return new FlipTileData
      {
        WideBackBackgroundImage = new Uri(string.Empty, UriKind.Relative),
        WideBackContent = description,
        BackBackgroundImage = new Uri(string.Empty, UriKind.Relative),
        BackContent = description,
        BackTitle = pokemonName
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
  }
}
