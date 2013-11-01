using System;
using System.Linq;
using HtmlAgilityPack;

namespace Schmogon
{
  public partial class SchmogonClient : ISchmogonClient
  {
    private const string SitePrefix = "http://www.smogon.com";

    private const string DescHeader = "Description";
    private const string CompHeader = "Competitive Use";

    private static string processUL(HtmlNode list)
    {
      if (list.Name.Equals("ul") != true) throw new ArgumentException("param must be of the node type UL", "list");

      return list.Elements("li").Aggregate(string.Empty, (current, elem) => current + ("* " + elem.InnerText.Trim() + "\n"));
    }
  }
}
