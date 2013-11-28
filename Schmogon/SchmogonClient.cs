using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Schmogon.Data;
using Schmogon.Model.Text;

namespace Schmogon
{
  public partial class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";

    public async Task<IEnumerable<T>> DeserializeSearchItemListAsync<T>(string serialized)
      where T : ISearchItem
    {
      return await JsonConvert.DeserializeObjectAsync<IEnumerable<T>>(serialized);
    }

    public async Task<string> SerializeSearchItemListAsync<T>(IEnumerable<T> searchItemList)
      where T : ISearchItem
    {
      return await JsonConvert.SerializeObjectAsync(searchItemList);
    }

    private static Paragraph processIntoParagraph(HtmlNode para)
    {
      if (!para.Name.Equals("p") && !para.Name.Equals("#text")) throw new ArgumentException("param must be of the node type P or #text", "para");

      return new Paragraph(sanitize(para.InnerText));
    }

    private static UnorderedList processIntoUnorderedList(HtmlNode list)
    {
      if (!list.Name.Equals("ul")) throw new ArgumentException("param must be of the node type UL", "list");

      var listElems = list.Elements("li")
        .Select(n => sanitize(n.InnerText));

      return new UnorderedList(listElems);
    }

    private static string sanitize(string s)
    {
      return WebUtility.HtmlDecode(s.Trim());
    }
  }
}
