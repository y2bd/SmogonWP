using System;
using System.Threading.Tasks;
using Schmogon.Model.Text;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertTextElementQuery = 
      "INSERT INTO TextElement (OwnerId, OwnerType, ElementType) VALUES (@ownerId, @ownerType, @elementType);";

    private const string InsertTextElementContentQuery =
      "INSERT INTO TextElementContent (Content, id_TextElement) VALUES (@content, @id_TextElement);";

    private async Task<long> insertTextElement(ITextElement element, string ownerId, OwnerType ownerType,
      ElementType elementType)
    {
      var statement = await _database.PrepareStatementAsync(InsertTextElementQuery);
      statement.BindTextParameterWithName("@ownerId", ownerId);
      statement.BindIntParameterWithName("@ownerType", (int)ownerType);
      statement.BindInt64ParameterWithName("@elementType", (int)elementType);

      await statement.StepAsync();

      var key = _database.GetLastInsertedRowId();

      var paragraph = element as Paragraph;
      if (paragraph != null) await insertTextElementContent(paragraph, key);
      else
      {
        var list = element as UnorderedList;
        if (list != null) await insertTextElementContent(list, key);
      }

      return key;
    }

    private async Task<long> insertTextElement(ITextElement element, long ownerId, OwnerType ownerType,
      ElementType elementType)
    {
      return await insertTextElement(element, ownerId.ToString(), ownerType, elementType);
    }

    private async Task<long> insertTextElementContent(Paragraph content, long textElementId)
    {
      var statement = await _database.PrepareStatementAsync(InsertTextElementContentQuery);
      statement.BindTextParameterWithName("@content", content.Content);
      statement.BindInt64ParameterWithName("@id_TextElement", textElementId);

      await statement.StepAsync();

      var key = _database.GetLastInsertedRowId();
      return key;
    }

    private async Task<long> insertTextElementContent(UnorderedList content, long textElementId)
    {
      foreach (var element in content.Elements)
      {
        var statement = await _database.PrepareStatementAsync(InsertTextElementContentQuery);
        statement.BindTextParameterWithName("@content", element);
        statement.BindInt64ParameterWithName("@id_TextElement", textElementId);

        await statement.StepAsync();
      }

      var key = _database.GetLastInsertedRowId();
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
