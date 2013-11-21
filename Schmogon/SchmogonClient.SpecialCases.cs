using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Schmogon.Data.Moves;
using Schmogon.Model.Text;
using Schmogon.Utilities;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    #region move cases
    private static readonly IDictionary<string, Func<HtmlNode, MoveDescriptions>> SpecialMoveDescriptionCases
      = new Dictionary<string, Func<HtmlNode, MoveDescriptions>>
      {
        {"Tri Attack", scrapeTriAttackDescription},
        {"Beat Up", scrapeBeatUpDescription}
      };

    private static MoveDescriptions scrapeTriAttackDescription(HtmlNode content)
    {
      // read scrapeMoveDescriptions (the general case) for an explanation on how scraping generally works

      var children = content.ChildNodes;

      var descIndex = children.FindIndex(h => h.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(h => h.InnerText.Trim().Equals(CompHeader));
      var relIndex = children.FindIndex(h => h.InnerText.Trim().Equals(RelMovesHeader));

      relIndex = relIndex == -1 ? children.Count : relIndex;
      compIndex = compIndex == -1 ? relIndex : compIndex;

      descIndex = descIndex == -1 ? children.FindIndex(n => n.Id.Equals("info")) : descIndex;

      var descParas = new List<ITextElement>();
      var compParas = new List<ITextElement>();
      var relMoves = new List<Move>();

      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // here's a change, look below for explanation
        if (!child.Name.Equals("p") && !child.Name.Equals("ul") && !child.Name.Equals("#text")) continue;

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

          if (element is Paragraph && string.IsNullOrEmpty(((Paragraph)element).Content)) continue;

          descParas.Add(element);
        }
        else if (i.IsBetween(compIndex, relIndex))
        {
          ITextElement element;

          if (child.Name.Equals("ul"))
          {
            element = processIntoUnorderedList(child);
          }
          else if (child.Name.Equals("#text"))
          {
            // here's the difference
            // the competitive text block for tri attack is actually in a #text node instead of a p node
            // because the smogon folks forgot to close a <p> tag
            element = processIntoParagraph(child);
          }
          else
          {
            element = processIntoParagraph(child);
          }

          // we're gonna get a lotta blank ones, so let's dispose of them
          if (element is Paragraph && string.IsNullOrEmpty(((Paragraph)element).Content)) continue;

          compParas.Add(element);
        }
        else if (i > relIndex)
        {
          var anchor = child.Element("a");

          if (anchor == null) continue;

          var name = anchor.InnerText.Trim();
          var desc = child.InnerText.Trim();
          var page = anchor.GetAttributeValue("href", null);

          relMoves.Add(new Move(name, desc, page));
        }
      }

      return new MoveDescriptions
      {
        Description = descParas,
        Competitive = compParas,
        RelatedMoves = relMoves
      };
    }

    private static MoveDescriptions scrapeBeatUpDescription(HtmlNode content)
    {
      // read scrapeMoveDescriptions (the general case) for an explanation on how scraping generally works

      var children = content.ChildNodes;

      var descIndex = children.FindIndex(h => h.InnerText.Trim().Equals(DescHeader));
      var compIndex = children.FindIndex(h => h.InnerText.Trim().Equals(CompHeader));
      var relIndex = children.FindIndex(h => h.InnerText.Trim().Equals(RelMovesHeader));

      relIndex = relIndex == -1 ? children.Count : relIndex;
      compIndex = compIndex == -1 ? relIndex : compIndex;

      descIndex = descIndex == -1 ? children.FindIndex(n => n.Id.Equals("info")) : descIndex;

      var descParas = new List<ITextElement>();
      var compParas = new List<ITextElement>();
      var relMoves = new List<Move>();

      for (int i = 0; i < children.Count; i++)
      {
        var child = children[i];

        // here's a change, look below for explanation
        if (!child.Name.Equals("p") && !child.Name.Equals("ul") && !child.Name.Equals("#text")) continue;

        if (i.IsBetween(descIndex, compIndex))
        {
          // funny enough, beat up's problem is solved the same way as tri attack
          // except it takes place in the description instead of the competitive
          // goddammit smogon close your paragraph tags
          ITextElement element;

          if (child.Name.Equals("ul"))
          {
            element = processIntoUnorderedList(child);
          }
          else if (child.Name.Equals("#text"))
          {
            element = processIntoParagraph(child);
          }
          else
          {
            element = processIntoParagraph(child);
          }

          // we're gonna get a lotta blank ones, so let's dispose of them
          // if it's a paragraph and there's no text inside, throw it away
          if (element is Paragraph && string.IsNullOrEmpty(((Paragraph)element).Content)) continue;

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

          if (element is Paragraph && string.IsNullOrEmpty(((Paragraph)element).Content)) continue;

          compParas.Add(element);
        }
        else if (i > relIndex)
        {
          var anchor = child.Element("a");

          if (anchor == null) continue;

          var name = anchor.InnerText.Trim();
          var desc = child.InnerText.Trim();
          var page = anchor.GetAttributeValue("href", null);

          relMoves.Add(new Move(name, desc, page));
        }
      }

      return new MoveDescriptions
      {
        Description = descParas,
        Competitive = compParas,
        RelatedMoves = relMoves
      };
    }
    #endregion move cases
  }
}
