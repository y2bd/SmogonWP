using System;
using System.Globalization;
using System.Threading.Tasks;
using SchmogonDB.Model.Text;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    private const string InsertTextElementQuery = 
      "INSERT INTO TextElement (OwnerId, OwnerType, ElementType) VALUES (@ownerId, @ownerType, @elementType);";

    private const string InsertTextElementContentQuery =
      "INSERT INTO TextElementContent (Content, id_TextElement) VALUES (@content, @id_TextElement);";

    private async Task<long> insertTextElement(Database database, ITextElement element, string ownerId, OwnerType ownerType,
      ElementType elementType)
    {
      var statement = await database.PrepareStatementAsync(InsertTextElementQuery);
      statement.BindTextParameterWithName("@ownerId", ownerId);
      statement.BindIntParameterWithName("@ownerType", (int)ownerType);
      statement.BindInt64ParameterWithName("@elementType", (int)elementType);

      await statement.StepAsync();

      var key = database.GetLastInsertedRowId();

      var paragraph = element as Paragraph;
      if (paragraph != null) await insertTextElementContent(database, paragraph, key);
      else
      {
        var list = element as UnorderedList;
        if (list != null) await insertTextElementContent(database, list, key);
      }

      return key;
    }

    private async Task<long> insertTextElement(Database database, ITextElement element, long ownerId, OwnerType ownerType,
      ElementType elementType)
    {
      return await insertTextElement(database, element, ownerId.ToString(CultureInfo.InvariantCulture), ownerType, elementType);
    }

    private async Task<long> insertTextElementContent(Database database, Paragraph content, long textElementId)
    {
      var statement = await database.PrepareStatementAsync(InsertTextElementContentQuery);
      statement.BindTextParameterWithName("@content", content.Content);
      statement.BindInt64ParameterWithName("@id_TextElement", textElementId);

      await statement.StepAsync();

      var key = database.GetLastInsertedRowId();
      return key;
    }

    private async Task<long> insertTextElementContent(Database database, UnorderedList content, long textElementId)
    {
      foreach (var element in content.Elements)
      {
        var statement = await database.PrepareStatementAsync(InsertTextElementContentQuery);
        statement.BindTextParameterWithName("@content", element);
        statement.BindInt64ParameterWithName("@id_TextElement", textElementId);

        await statement.StepAsync();
      }

      var key = database.GetLastInsertedRowId();
      return key;
    }
  }

  public enum OwnerType
  {
    Ability,
    Item,
    Pokemon,
    Move,
    Moveset
  }

  public enum ElementType
  {
    Description,
    Competitive,
    Options,
    Counters
  }
}
