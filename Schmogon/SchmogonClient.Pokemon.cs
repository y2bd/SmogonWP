using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Schmogon.Data.Abilities;
using Schmogon.Data.Pokemon;
using Schmogon.Data.Stats;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    public const string PokemonSearch = "http://www.smogon.com/bw/pokemon/";

    private IEnumerable<Pokemon> _pokemonCache;

    public async Task<IEnumerable<Pokemon>> GetAllPokemonAsync()
    {
      return _pokemonCache ?? (_pokemonCache = await getAllPokemon());
    }

    private async Task<IEnumerable<Pokemon>> getAllPokemon()
    {
      var doc = await fetchPokemonDoc();

      var table = doc.DocumentNode.Descendants("table")
        .First(n => n.Id.Contains("pokemon_list"));

      var tbody = table.Element("tbody");

      // isnt page scraping fun?
      var pokemon = (from row in tbody.Descendants("tr")
                     select row.Descendants("td")
                       into data
                       let name = data.ElementAt(0).InnerText.Trim()
                       let path = data.ElementAt(0).Element("a").GetAttributeValue("href", "")
                       let types = processTypesNode(data.ElementAt(1))
                       let tier = processTierNode(data.ElementAt(2))
                       let abilities = processAbilitiesNode(data.ElementAt(3))
                       let hp = int.Parse(data.ElementAt(4).InnerText.Trim())
                       let atk = int.Parse(data.ElementAt(5).InnerText.Trim())
                       let def = int.Parse(data.ElementAt(6).InnerText.Trim())
                       let spa = int.Parse(data.ElementAt(7).InnerText.Trim())
                       let spd = int.Parse(data.ElementAt(8).InnerText.Trim())
                       let spe = int.Parse(data.ElementAt(9).InnerText.Trim())
                       let stats = new BaseStat(hp, atk, def, spa, spd, spe)
                       select new Pokemon(name, types, tier, abilities, stats, path)).ToList();

      return pokemon;
    }

    private IEnumerable<Type> processTypesNode(HtmlNode tr)
    {
      var types = new List<Type>();

      foreach (var anchor in tr.Descendants("a"))
      {
        Type type;

        Enum.TryParse(anchor.InnerText.Trim(), true, out type);

        types.Add(type);
      }

      return types;
    }

    private Tier processTierNode(HtmlNode tr)
    {
      var anchor = tr.Descendants("a").First();

      Tier tier;

      Enum.TryParse(anchor.InnerText.Trim(), true, out tier);

      return tier;
    }

    private IEnumerable<Ability> processAbilitiesNode(HtmlNode tr)
    {
      var abilities = (from anchor in tr.Descendants("a") 
                       let name = anchor.InnerText.Trim() 
                       let path = anchor.GetAttributeValue("href", "") 
                       select new Ability(name, string.Empty, path))
                      .ToList();

      return abilities;
    }

    // we need to make a special httpclient, can't just use standard
    private async Task<HtmlDocument> fetchPokemonDoc()
    {
      var baseAddress = new Uri(SitePrefix);
      var cookies = new CookieContainer();

      HtmlDocument result;

      using (var handler = new HttpClientHandler { CookieContainer = cookies })
      using (var client = new HttpClient(handler))
      {
        cookies.Add(baseAddress, new Cookie("dexprefs", "AQAAAA=="));

        var answer = await client.GetStringAsync(new Uri(PokemonSearch));

        result = new HtmlDocument();
        result.LoadHtml(answer);
      }

      return result;
    }

    #region serialization

    public async Task<string> SerializePokemonListAsync()
    {
      var pokemon = await GetAllPokemonAsync();

      var cereal = await JsonConvert.SerializeObjectAsync(pokemon);

      return cereal;
    }

    public async Task<IEnumerable<Pokemon>> DeserializePokemonListAsync(string pokemon)
    {
      return (_pokemonCache = await JsonConvert.DeserializeObjectAsync<IEnumerable<Pokemon>>(pokemon));
    }

    #endregion serialization
  }
}