using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Schmogon.Utilities;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    public const string PokemonSearch = "http://www.smogon.com/bw/pokemon/";
    public const string MovesSuffix = "/moves";

    public const string OverviewHeader = "Overview";
    public const string OptionsHeader = "Other Options";
    public const string CountersHeader = "Checks and Counters";

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

    public async Task<PokemonData> GetPokemonDataAsync(Pokemon pokemon)
    {
      var descTask = getPokemonDescriptions(pokemon);
      var moveTask = getPokemonMoves(pokemon);
      await Task.WhenAll(descTask, moveTask);

      var desc = await descTask;
      var moves = await moveTask;

      var pokedata = new PokemonData(
        pokemon.Name,
        pokemon.PageLocation,
        desc.SpritePath,
        pokemon.Abilities,
        pokemon.Types,
        pokemon.Tier,
        pokemon.BaseStats,
        desc.Overview, // overview
        desc.Movesets,
        desc.Options, // other options
        desc.Counters, // counters
        moves);

      return pokedata;
    }

    private async Task<PokemonDescriptions> getPokemonDescriptions(Pokemon pokemon)
    {
      var path = SitePrefix + pokemon.PageLocation;

      var doc = await Web.GetDocumentAsync(path);

      var content = doc.DocumentNode.Descendants("div").First(d => d.Id.Equals("content_wrapper"));

      var desc = processDescriptions(content);

      desc.SpritePath = processSpritePath(content);

      return desc;
    }

    private async Task<IEnumerable<Move>> getPokemonMoves(Pokemon pokemon)
    {
      var path = SitePrefix + pokemon.PageLocation + "/moves";

      var doc = await Web.GetDocumentAsync(path);

      var content = doc.DocumentNode.Descendants("table").First(t => t.Id.Contains("move_list")).Element("tbody");

      var moves = processMoveList(content);

      return moves;
    }

    private PokemonDescriptions processDescriptions(HtmlNode content)
    {
      var nodes = content.ChildNodes.Where(n => !n.Name.Equals("#text")).ToList();

      var overIndex = nodes.FindIndex(n => n.InnerText.Trim().Equals(OverviewHeader));
      var movesIndex = nodes.FindIndex(n => n.Name.Equals("a"));
      var optionsIndex = nodes.FindIndex(n => n.InnerText.Trim().Equals(OptionsHeader));
      var countersIndex = nodes.FindIndex(n => n.InnerText.Trim().Equals(CountersHeader));

      countersIndex = countersIndex == -1 ? nodes.Count : countersIndex;
      optionsIndex = optionsIndex == -1 ? countersIndex : optionsIndex;
      movesIndex = movesIndex == -1 ? optionsIndex : movesIndex;
      overIndex = overIndex == -1 ? movesIndex : overIndex;

      var overview = new List<ITextElement>();
      var movesets = new List<Moveset>();
      var options = new List<ITextElement>();
      var counters = new List<ITextElement>();

      Moveset currentMoveset = null;

      for (int i = 0; i < nodes.Count; i++)
      {
        var node = nodes[i];

        if (i.IsBetween(overIndex, movesIndex))
        {
          processGeneralDescriptionBlock(node, overview);
        }
        else if (i.IsBetween(movesIndex, optionsIndex - 1, true))
        {
          currentMoveset = processMovesetBlock(node, currentMoveset, movesets);
        }
        else if (i.IsBetween(optionsIndex, countersIndex))
        {
          processGeneralDescriptionBlock(node, options);
        }
        else if (i.IsBetween(countersIndex, nodes.Count))
        {
          processGeneralDescriptionBlock(node, counters);
        }
      }

      // add the last one
      if (currentMoveset != null) movesets.Add(currentMoveset);

      return new PokemonDescriptions
      {
        Overview = overview,
        Movesets = movesets,
        Options = options,
        Counters = counters
      };
    }

    private Moveset processMovesetBlock(HtmlNode node, Moveset currentMoveset, ICollection<Moveset> movesets)
    {
      // empty anchors seperate the moves sets thank god
      if (node.Name.Equals("a") && string.IsNullOrWhiteSpace(node.InnerText))
      {
        // time for a new moveset!
        // add the old one first if it is one
        if (currentMoveset != null) movesets.Add(currentMoveset);

        currentMoveset = new Moveset();
      }
      else if (node.Name.Equals("table") && hasClass(node, "strategyheader"))
      {
        var tds = node.Descendants("tr").Last().Elements("td").ToList();

        currentMoveset = currentMoveset ?? new Moveset();

        currentMoveset.Name = tds[0].InnerText.Trim();
        currentMoveset.Items = processItemsNode(tds[1]);

        // sometimes they don't have abilities listed for some reason
        if (tds.Count <= 3)
        {
          currentMoveset.Natures = processNaturesNode(tds[2]);
        }
        else
        {
          currentMoveset.Abilities = processAbilitiesNode(tds[2]);
          currentMoveset.Natures = processNaturesNode(tds[3]);
        }
      }
      else if (node.Name.Equals("table") && hasClass(node, "moveset"))
      {
        currentMoveset = currentMoveset ?? new Moveset();

        // sometimes they need to show IVs as well, usually for hidden power
        // that's why we have two cases
        var trs = node.Descendants("tr").ToArray();

        if (trs.Length == 4)
        {
          var moveRow = trs[1].Elements("td").ToArray();

          currentMoveset.Moves = processMovesNode(moveRow[0]);

          var evRow = trs[3].Elements("td").ToArray();

          currentMoveset.EVSpread = processEVNode(evRow[0]);
        }
        else if (trs.Length == 2)
        {
          var row = trs[1].Elements("td").ToArray();

          currentMoveset.Moves = processMovesNode(row[0]);
          currentMoveset.EVSpread = processEVNode(row[1]);
        }
      }
      else if (node.Name.Equals("p"))
      {
        currentMoveset.Description = currentMoveset.Description ?? new List<ITextElement>();

        processGeneralDescriptionBlock(node, currentMoveset.Description);
      }
      else if (node.Name.Equals("div"))
      {
        var paras = node.Elements("p").Concat(node.Elements("ul"));

        currentMoveset.Description = currentMoveset.Description ?? new List<ITextElement>();

        foreach (var para in paras)
        {
          processGeneralDescriptionBlock(para, currentMoveset.Description);
        }
      }

      return currentMoveset;
    }

    private void processGeneralDescriptionBlock(HtmlNode node, ICollection<ITextElement> elementList)
    {
      if (node.Name.Equals("ul"))
      {
        elementList.Add(processIntoUnorderedList(node));
      }
      else if (node.Name.Equals("p"))
      {
        elementList.Add(processIntoParagraph(node));
      }
    }

    private string processSpritePath(HtmlNode content)
    {
      // get me the first <td class="sprite" />
      var td =
        content.Descendants("td")
          .First(n => hasClass(n, "sprite"));

      var imgSrc = td.Element("img").Attributes["src"].Value;

      return imgSrc;
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

    private IEnumerable<Item> processItemsNode(HtmlNode td)
    {
      return td.Descendants("a")
               .Select(node => new Item(node.InnerText.Trim(), "", node.GetAttributeValue("href", "")))
               .ToList();
    }

    private IEnumerable<Nature> processNaturesNode(HtmlNode td)
    {
      var natures = new List<Nature>();

      foreach (var anchor in td.Descendants("a"))
      {
        Nature nature;

        Enum.TryParse(anchor.InnerText.Trim(), true, out nature);

        natures.Add(nature);
      }

      return natures;
    }

    private IEnumerable<IEnumerable<Move>> processMovesNode(HtmlNode td)
    {
      // this one is pretty tricky because their formatting for this list is stupid
      var nodes = td.ChildNodes.Where(n => !n.Name.Equals("#text")).ToList();

      var moves = new List<List<Move>>();

      int moveCounter = 0;

      // every time we encounter a br, it's time for a new move collection
      foreach (var node in nodes)
      {
        if (node.Name.Equals("a"))
        {
          var move = new Move(
            node.InnerText.Trim(),
            string.Empty,
            node.GetAttributeValue("href", string.Empty));

          if (moves.Count < moveCounter + 1)
          {
            moves.Add(new List<Move>());
          }

          moves[moveCounter].Add(move);
        }
        else
        {
          moveCounter++;
        }
      }

      return moves;
    }

    private BaseStat processEVNode(HtmlNode td)
    {
      var str = td.InnerText.Trim();
      var parts = str.Split('/').Select(s => s.Trim()).Select(s => s.Split(' '));

      var baseStat = new BaseStat();

      foreach (var part in parts)
      {
        switch (part[1])
        {
          case "HP":
            baseStat.HP = int.Parse(part[0]);
            break;
          case "Atk":
            baseStat.Attack = int.Parse(part[0]);
            break;
          case "Def":
            baseStat.Defense = int.Parse(part[0]);
            break;
          case "SpA":
            baseStat.SpecialAttack = int.Parse(part[0]);
            break;
          case "SpD":
            baseStat.SpecialDefense = int.Parse(part[0]);
            break;
          case "Spe":
            baseStat.Speed = int.Parse(part[0]);
            break;
        }
      }

      return baseStat;
    }

    private IEnumerable<Move> processMoveList(HtmlNode tbody)
    {
      var rows = tbody.Elements("tr");

      var moves = (from row in rows
                   select row.Elements("td").ToList()
                     into tds
                     let name = tds[0].InnerText.Trim()
                     let description = tds[5].InnerText.Trim()
                     let path = tds[0].Element("a").GetAttributeValue("href", string.Empty)
                     select new Move(name, description, path));

      return moves.ToList();
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

    private static bool hasClass(HtmlNode node, string className)
    {
      return node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains(className);
    }
  }

  internal class PokemonDescriptions
  {
    public IEnumerable<ITextElement> Overview { get; set; }
    public IEnumerable<Moveset> Movesets { get; set; }
    public IEnumerable<ITextElement> Options { get; set; }
    public IEnumerable<ITextElement> Counters { get; set; }
    public string SpritePath { get; set; }
  }
}