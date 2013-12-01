using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using Schmogon.Data.Items;
using Schmogon.Data.Pokemon;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string InsertItemQuery = "INSERT INTO Item VALUES (@name);";

    private const string InsertItemToMovesetQuery =
      "INSERT INTO ItemToMoveset (Name_Item, id_Moveset) VALUES (@item, @moveset);";

    private async Task<string> insertItemData(ItemData item)
    {
      var statement = await _database.PrepareStatementAsync(InsertItemQuery);
      statement.BindTextParameterWithName("@name", item.Name);

      await statement.StepAsync();

      foreach (var desc in item.Description)
      {
        await insertTextElement(desc, item.Name, OwnerType.Item, ElementType.Description);
      }

      foreach (var comp in item.Competitive)
      {
        await insertTextElement(comp, item.Name, OwnerType.Item, ElementType.Competitive);
      }

      return item.Name;
    }

    private async Task<long> insertMovesetItemConnections(Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var item in moveset.Items)
      {
        var statement = await _database.PrepareStatementAsync(InsertItemToMovesetQuery);
        statement.BindTextParameterWithName("@item", item.Name);
        statement.BindInt64ParameterWithName("@moveset", movesetId);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception e)
        {
          continue;
        }

        lastKey = _database.GetLastInsertedRowId();
      }

      return lastKey;
    }
  }
}
