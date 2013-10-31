using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Schmogon.Data.Abilities;
using Schmogon.Utilities;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    private const string AbilitySearch = "http://www.smogon.com/bw/abilities/";

    private IEnumerable<Ability> _abilityCache;

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

    public async Task<AbilityData> GetAbilityDataAsync(Ability ability)
    {
      var path = SitePrefix + ability.PageLocation;

      var doc = await Web.GetDocumentAsync(path);

      var content = doc.DocumentNode.Descendants("div").First(d => d.Id.Equals("content_wrapper"));

      var descParts = scrapeAbilityDescription(content);

      return new AbilityData(
        WebUtility.HtmlDecode(ability.Name),
        WebUtility.HtmlDecode(descParts.Item1),
        WebUtility.HtmlDecode(descParts.Item2));
    }

    private static Tuple<string, string> scrapeAbilityDescription(HtmlNode content)
    {
      var children = content.ChildNodes;

      var descIndex = children.FindIndex(n => n.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(n => n.InnerText.Trim().Equals(CompHeader));

      var descParas = new List<String>();
      var compParas = new List<String>();

      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // we only want matching paragraphs or unordered lists
        if (!child.Name.Equals("p")) continue;

        if (i.IsBetween(descIndex, compIndex))
        {
          descParas.Add(child.InnerText.Trim());
        }
        else if (i.IsBetween(compIndex, children.Count))
        {
          compParas.Add(child.InnerText.Trim());
        }
      }

      return new Tuple<string, string>(String.Join("\n\n", descParas), String.Join("\n\n", compParas));
    }
  }
}
