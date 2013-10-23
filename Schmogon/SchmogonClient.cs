using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Schmogon.Data.Abilities;
using Schmogon.Data.Moves;
using Schmogon.Utilities;

namespace Schmogon
{
  public class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";
    private const string MoveSearch = "http://www.smogon.com/bw/moves/";
    private const string AbilitySearch = "http://www.smogon.com/bw/abilities/";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";
    private const string RelHeader = "Related Moves";

    private IEnumerable<Move> _moveCache;
    private IEnumerable<Ability> _abilityCache;

    #region moves

    public async Task<IEnumerable<Move>> GetAllMovesAsync()
    {
      return _moveCache ?? (_moveCache = await getAllMoves());
    }

    public async Task<IEnumerable<Move>> SearchMovesAsync(string query)
    {
      query = query.Trim().ToLowerInvariant();

      var moves = await GetAllMovesAsync();

      var res = moves.Where(m => m.Name.ToLowerInvariant().Contains(query));

      return res;
    }

    private async Task<IEnumerable<Move>> getAllMoves()
    {
      var doc = await Web.GetDocumentAsync(MoveSearch);

      var table = doc.DocumentNode.Descendants("table")
        .First(n => n.Id.Contains("move_list"));

      var tbody = table.Descendants("tbody").First();

      var moves = (from row in tbody.Descendants("tr")
                   select row.Descendants("td") into data
                     let name = data.ElementAt(0).InnerText.Trim()
                     let desc = data.ElementAt(5).InnerText.Trim()
                     let path = data.ElementAt(0).Element("a").GetAttributeValue("href", "")
                     select new Move(name, desc, path))
                  .ToList();

      return moves;
    }

    public async Task<MoveData> GetMoveDataAsync(Move move)
    {
      var path = SitePrefix + move.PageLocation;

      var doc = await Web.GetDocumentAsync(path);

      var content = doc.DocumentNode.Descendants("div").First(d => d.Id.Equals("content_wrapper"));

      // first get the stats
      var stats = scrapeMoveStats(content.Element("table"));

      // now get the text parts
      Tuple<string, string, IEnumerable<Move>> descParts = scrapeMoveDescriptions(content);

      return new MoveData(
        WebUtility.HtmlDecode(move.Name),
        stats,
        WebUtility.HtmlDecode(descParts.Item1),
        WebUtility.HtmlDecode(descParts.Item2),
        descParts.Item3);
    }

    private static MoveStats scrapeMoveStats(HtmlNode statTable)
    {
      var tds = statTable.Descendants("td").ToList();

      var type = tds[0].InnerText.Trim();
      var power = tds[1].InnerText.Trim();
      var acc = tds[2].InnerText.Trim();
      var pp = tds[3].InnerText.Trim();
      var prio = tds[4].InnerText.Trim();
      var dam = tds[5].InnerText.Trim();
      var tar = tds[6].InnerText.Trim();

      var stats = new MoveStats(type, power, acc, pp, prio, dam, tar);

      return stats;
    }

    private static Tuple<string, string, IEnumerable<Move>> scrapeMoveDescriptions(HtmlNode content)
    {
      // we need access to the children so we can get indices
      var children = content.ChildNodes;

      // find the indices of the headers so we know where the relevant paragraphs lie
      var descIndex = children.FindIndex(h => h.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(h => h.InnerText.Trim().Equals(CompHeader));
      var relIndex = children.FindIndex(h => h.InnerText.Trim().Equals(RelHeader));

      // sometimes there aren't sections, this is a neat way to just ignore non-existant ones
      // we fold over the non-existant ones
      relIndex = relIndex == -1 ? children.Count : relIndex;
      compIndex = compIndex == -1 ? relIndex : compIndex;

      // sometimes for whatever reason move pages don't have the description header
      // so instead we'll make the stat table the description header
      descIndex = descIndex == -1 ? children.FindIndex(n => n.Id.Equals("info")) : descIndex;

      var descParas = new List<String>();
      var compParas = new List<String>();
      var relMoves = new List<Move>();

      // now cycle through all of the child nodes
      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // we only want matching paragraphs or unordered lists
        if (!child.Name.Equals("p") && !child.Name.Equals("ul")) continue;

        if (i.IsBetween(descIndex, compIndex))
        {
          // sometimes smogon embeds lists into descriptions
          // these are usually used for listing abilities that may interact with the move
          // we need to convert those to plaintext for now
          // in the future they might be parsed into seperate objects that the client can recognize
          descParas.Add(child.Name.Equals("ul") ? processUL(child).Trim() : child.InnerText.Trim());
        }
        else if (i.IsBetween(compIndex, relIndex))
        {
          compParas.Add(child.Name.Equals("ul") ? processUL(child).Trim() : child.InnerText.Trim());
        }
        else if (i > relIndex)
        {
          // parse fix!
          // sometimes there are bits in the related index that aren't related moves, but commentary
          // like on extremespeed
          // they don't have anchors, so only add ones that have anchors
          var anchor = child.Element("a");

          if (anchor == null) continue;

          var name = anchor.InnerText.Trim();
          var desc = child.InnerText.Trim();
          var page = anchor.GetAttributeValue("href", null);

          relMoves.Add(new Move(name, desc, page));
        }
      }

      return new Tuple<string, string, IEnumerable<Move>>(String.Join("\n\n", descParas), String.Join("\n\n", compParas), relMoves);
    }

    #endregion moves

    #region abilities

    public async Task<IEnumerable<Ability>> GetAllAbilitiesAsync()
    {
      return _abilityCache ?? (_abilityCache = await getAllAbilities());
    }

    public async Task<IEnumerable<Ability>> SearchAbilitiesAsync(string query)
    {
      query = query.Trim().ToLowerInvariant();

      var abilities = await GetAllAbilitiesAsync();

      var res = abilities.Where(m => m.Name.ToLowerInvariant().Contains(query));

      return res;
    }

    private async Task<IEnumerable<Ability>> getAllAbilities()
    {
      var doc = await Web.GetDocumentAsync(AbilitySearch);

      var table = doc.DocumentNode.Descendants("table")
        .First(n => n.Id.Contains("ability_list"));

      var tbody = table.Element("tbody");

      var abilities = (from row in tbody.Descendants("tr")
                       select row.Descendants("td") into data
                         let name = data.ElementAt(0).InnerText.Trim()
                         let desc = data.ElementAt(1).InnerText.Trim()
                         let path = data.ElementAt(0).Element("a").GetAttributeValue("href", "")
                         select new Ability(name, desc, path))
                      .ToList();

      return abilities;
    }

    public Task<AbilityData> GetAbilityDataAsync(Ability ability)
    {
      throw new NotImplementedException();
    }

    #endregion abilities

    private static string processUL(HtmlNode list)
    {
      if (list.Name.Equals("ul") != true) throw new ArgumentException("param must be of the node type UL", "list");

      return list.Elements("li").Aggregate(string.Empty, (current, elem) => current + ("* " + elem.InnerText.Trim() + "\n"));
    }

  }
}
