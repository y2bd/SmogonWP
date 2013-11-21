using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Schmogon.Data.Items;
using Schmogon.Model.Text;
using Schmogon.Utilities;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    private const string ItemSearch = "http://www.smogon.com/bw/items/";
    public const string LocationHeader = "Location";

    private IEnumerable<Item> _itemCache;

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
      return _itemCache ?? (_itemCache = await getAllitems());
    }

    public async Task<IEnumerable<Item>> SearchItemsAsync(string query)
    {
      query = query.Trim().ToLowerInvariant();

      var items = await GetAllItemsAsync();

      var res = items.Where(m => m.Name.ToLowerInvariant().Contains(query));

      return res;
    }

    private async Task<IEnumerable<Item>> getAllitems()
    {
      var doc = await Web.GetDocumentAsync(ItemSearch);

      var table = doc.DocumentNode.Descendants("table")
        .First(n => n.Id.Contains("item_list"));

      var tbody = table.Element("tbody");

      var items = (from row in tbody.Descendants("tr")
                       select row.Descendants("td") into data
                       let name = data.ElementAt(0).InnerText.Trim()
                       let desc = data.ElementAt(1).InnerText.Trim()
                       let path = data.ElementAt(0).Element("a").GetAttributeValue("href", "")
                       select new Item(name, desc, path))
                      .ToList();

      return items;
    }

    public async Task<ItemData> GetItemDataAsync(Item item)
    {
      var path = SitePrefix + item.PageLocation;

      var doc = await Web.GetDocumentAsync(path);

      var content = doc.DocumentNode.Descendants("div").First(d => d.Id.Equals("content_wrapper"));

      var descParts = scrapeItemDescription(content);

      return new ItemData(
        WebUtility.HtmlDecode(item.Name),
        descParts.Item1,
        descParts.Item2);
    }

    private static Tuple<IEnumerable<ITextElement>, IEnumerable<ITextElement>> scrapeItemDescription(HtmlNode content)
    {
      var children = content.ChildNodes;

      var descIndex = children.FindIndex(n => n.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(n => n.InnerText.Trim().Equals(CompHeader));
      var loctIndex = children.FindIndex(n => n.InnerText.Trim().Equals(LocationHeader));

      var descParas = new List<ITextElement>();
      var compParas = new List<ITextElement>();

      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // we only want matching paragraphs or unordered lists
        if (!child.Name.Equals("p") && !child.Name.Equals("ul")) continue;

        if (i.IsBetween(descIndex, compIndex))
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

          descParas.Add(element);
        }
        else if (i.IsBetween(compIndex, loctIndex))
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
      }

      return new Tuple<IEnumerable<ITextElement>, IEnumerable<ITextElement>>(descParas, compParas);
    }

    #region serialization

    public async Task<string> SerializeItemListAsync()
    {
      var items = await GetAllItemsAsync();

      var cereal = await JsonConvert.SerializeObjectAsync(items);

      return cereal;
    }

    public async Task<IEnumerable<Item>> DeserializeItemListAsync(string items)
    {
      return (_itemCache = await JsonConvert.DeserializeObjectAsync<IEnumerable<Item>>(items));
    }

    #endregion serialization
  }
}
