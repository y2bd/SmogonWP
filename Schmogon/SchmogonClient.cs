using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Schmogon.Data;

namespace Schmogon
{
  public class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";
    private const string MoveSearch = "http://www.smogon.com/bw/moves/";

    private IEnumerable<Move> _moveCache; 

    public async Task<IEnumerable<Move>> SearchMovesAsync(string query)
    {
      query = query.Trim().ToLowerInvariant();

      if (_moveCache == null)
      {
        _moveCache = await getAllMoves();
      }

      var res = _moveCache.Where(m => m.Name.ToLowerInvariant().Contains(query));

      return res;
    }

    private async Task<IEnumerable<Move>> getAllMoves()
    {
      var moves = new List<Move>();

      var hc = new HttpClient();

      var stream = await hc.GetStreamAsync(MoveSearch);

      var doc = new HtmlDocument();
      doc.Load(stream);

      var table = doc.DocumentNode.Descendants("table")
        .First(n => n.Id.Contains("move_list"));

      var tbody = table.Descendants("tbody").First();

      foreach (var row in tbody.Descendants("tr"))
      {
        var data = row.Descendants("td");

        var name = data.ElementAt(0).InnerText.Trim();
        var desc = data.Last().InnerText.Trim();
        var path = data.ElementAt(0).Descendants("a").First().Attributes["href"].Value;
        
        moves.Add(new Move(name, desc, path));
      }
      
      hc.Dispose();

      return moves;
    }
  }
}
