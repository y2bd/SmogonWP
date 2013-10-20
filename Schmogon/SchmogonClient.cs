﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Schmogon.Data.Moves;
using Schmogon.Utilities;

namespace Schmogon
{
  public class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";
    private const string MoveSearch = "http://www.smogon.com/bw/moves/";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";
    private const string RelHeader = "Related Moves";

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

    public async Task<MoveData> GetMoveDataAsync(Move move)
    {
      var path = SitePrefix + move.PageLocation;

      var hc = new HttpClient();

      var stream = await hc.GetStreamAsync(path);

      var doc = new HtmlDocument();
      doc.Load(stream);

      var content = doc.DocumentNode.Descendants("div").First(d => d.Id.Equals("content_wrapper"));
      var children = content.ChildNodes;

      // first get the stats
      var table = content.Element("table");
      var tds = table.Descendants("td").ToList();

      var type = tds[0].InnerText.Trim();
      var power = tds[1].InnerText.Trim();
      var acc = tds[2].InnerText.Trim();
      var pp = tds[3].InnerText.Trim();
      var prio = tds[4].InnerText.Trim();
      var dam = tds[5].InnerText.Trim();
      var tar = tds[6].InnerText.Trim();

      var stats = new MoveStats(type, power, acc, pp, prio, dam, tar);

      // now get the text parts
      var descIndex = children.FindIndex(h => h.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(h => h.InnerText.Trim().Equals(CompHeader));
      var relIndex = children.FindIndex(h => h.InnerText.Trim().Equals(RelHeader));

      // sometimes there aren't sections, this is a neat way to just ignore non-existant ones
      // we fold over the non-existant ones
      relIndex = relIndex == -1 ? children.Count : relIndex;
      compIndex = compIndex == -1 ? relIndex : compIndex;
      descIndex = descIndex == -1 ? compIndex : descIndex;

      var descParas = new List<String>();
      var compParas = new List<String>();

      var relMoves = new List<Move>();

      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // we only want matching paragraphs
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
          var name = child.Element("a").InnerText.Trim();
          var desc = child.InnerText.Trim();
          var page = child.Element("a").GetAttributeValue("href", null);

          relMoves.Add(new Move(name, desc, page));
        }
      }

      return new MoveData(move.Name, stats, String.Join("\n\n", descParas), String.Join("\n\n", compParas), relMoves);
    }

    private string processUL(HtmlNode list)
    {
      return list.Elements("li").Aggregate(string.Empty, (current, elem) => current + ("* " + elem.InnerText.Trim() + "\n"));
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
        var desc = data.ElementAt(5).InnerText.Trim();
        var path = data.ElementAt(0).Descendants("a").First().Attributes["href"].Value;
        
        moves.Add(new Move(name, desc, path));
      }
      
      hc.Dispose();

      return moves;
    }
  }
}
