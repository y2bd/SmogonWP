using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchmogonDB.Model.Text;
using SchmogonDB.Population;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const char TextElementDelimiter = '@';

    private const string FetchTextElementsQuery =
      @"SELECT GROUP_CONCAT(tec.Content, '@') as Content 
        FROM TextElement te
        INNER JOIN TextElementContent tec ON tec.id_TextElement = te.id
        WHERE te.OwnerId = @ownerId 
          AND te.OwnerType = @ownerType 
          AND te.ElementType = @elementType
        GROUP BY te.id";

    private async Task<IEnumerable<ITextElement>> fetchTextElements(
      string ownerId, 
      OwnerType ownerType,
      ElementType elementType)
    {
      var textElements = new List<ITextElement>();

      var statement = await _database.PrepareStatementAsync(FetchTextElementsQuery);
      statement.BindTextParameterWithName("@ownerId", ownerId);
      statement.BindIntParameterWithName("@ownerType", (int)ownerType);
      statement.BindIntParameterWithName("@elementType", (int)elementType);

      while (statement.StepSync())
      {
        var contentString = statement.GetTextAt(0);

        if (contentString.Contains(TextElementDelimiter))
        {
          var elements = contentString.Split(TextElementDelimiter);

          textElements.Add(new UnorderedList(elements));
        }
        else
        {
          textElements.Add(new Paragraph(contentString));
        }
      }

      return textElements;
    }
  }
}
