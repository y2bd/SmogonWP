using System;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Schmogon.Model.Text;

namespace Schmogon
{
  public partial class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";
    
    private static Paragraph processIntoParagraph(HtmlNode para)
    {
      if (!para.Name.Equals("p")) throw new ArgumentException("param must be of the node type P", "para");

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
