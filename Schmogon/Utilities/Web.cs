using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Schmogon.Utilities
{
  public static class Web
  {
    public static HttpClient MakeHttpClient()
    {
      var hc = new HttpClient();

      hc.DefaultRequestHeaders.UserAgent.ParseAdd("Schmogon Scraper (//github.com/y2bd/schmogon)");

      return hc;
    }

    public static async Task<HtmlDocument> GetDocumentAsync(string uri)
    {
      var hc = MakeHttpClient();

      var doc = new HtmlDocument();
      doc.LoadHtml(await hc.GetStringAsync(uri));

      hc.Dispose();

      return doc;
    }
  }
}
