using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Schmogon.Data.Moves;
using Schmogon.Model.Text;
using Schmogon.Utilities;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    private const string MoveSearch = "http://www.smogon.com/bw/moves/";
    private const string MoveTypeSearch = "http://www.smogon.com/bw/types/";
    
    private const string RelMovesHeader = "Related Moves";

    private IEnumerable<Move> _moveCache;
    private IDictionary<Type, IEnumerable<Move>> _typedMoveCache; 
    
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

    public async Task<IEnumerable<Move>> GetMovesOfTypeAsync(Type type)
    {
      _typedMoveCache = _typedMoveCache ?? new Dictionary<Type, IEnumerable<Move>>();

      if (!_typedMoveCache.ContainsKey(type))
      {
        _typedMoveCache.Add(type, await getMovesOfType(type));
      }

      return _typedMoveCache[type];
    }

    public async Task<IEnumerable<Move>> SearchMovesOfTypeAsync(Type type, string query)
    {
      query = query.Trim().ToLowerInvariant();

      var moves = await GetMovesOfTypeAsync(type);

      var res = moves.Where(m => m.Name.ToLowerInvariant().Contains(query));

      return res;
    }

    private async Task<IEnumerable<Move>> getMovesOfType(Type type)
    {
      var page = MoveTypeSearch + Enum.GetName(typeof(Type), type).ToLower();

      var doc = await Web.GetDocumentAsync(page);

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
      Tuple<IEnumerable<ITextElement>, IEnumerable<ITextElement>, IEnumerable<Move>> descParts = scrapeMoveDescriptions(content);

      return new MoveData(
        WebUtility.HtmlDecode(move.Name),
        stats,
        descParts.Item1,
        descParts.Item2,
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

    private static Tuple<IEnumerable<ITextElement>, IEnumerable<ITextElement>, IEnumerable<Move>> scrapeMoveDescriptions(HtmlNode content)
    {
      // we need access to the children so we can get indices
      var children = content.ChildNodes;

      // find the indices of the headers so we know where the relevant paragraphs lie
      var descIndex = children.FindIndex(h => h.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(h => h.InnerText.Trim().Equals(CompHeader));
      var relIndex = children.FindIndex(h => h.InnerText.Trim().Equals(RelMovesHeader));

      // sometimes there aren't sections, this is a neat way to just ignore non-existant ones
      // we fold over the non-existant ones
      relIndex = relIndex == -1 ? children.Count : relIndex;
      compIndex = compIndex == -1 ? relIndex : compIndex;

      // sometimes for whatever reason move pages don't have the description header
      // so instead we'll make the stat table the description header
      descIndex = descIndex == -1 ? children.FindIndex(n => n.Id.Equals("info")) : descIndex;

      var descParas = new List<ITextElement>();
      var compParas = new List<ITextElement>();
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
          // descParas.Add(child.Name.Equals("ul") ? processUL(child).Trim() : child.InnerText.Trim());

          // this is the future, they are now parsed into seperate elements!
          ITextElement element;

          if (child.Name.Equals("ul"))
          {
            element = processIntoUnorderedList(child);
          }
          else
          {
            element = processIntoParagraph(child);
          }

          descParas.Add(element);
        }
        else if (i.IsBetween(compIndex, relIndex))
        {
          ITextElement element;

          if (child.Name.Equals("ul"))
          {
            element = processIntoUnorderedList(child);
          }
          else
          {
            element = processIntoParagraph(child);
          }

          compParas.Add(element);
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

      return new Tuple<IEnumerable<ITextElement>, IEnumerable<ITextElement>, IEnumerable<Move>>(descParas, compParas, relMoves);
    }
  }
}
