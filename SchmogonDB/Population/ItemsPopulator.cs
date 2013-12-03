using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Schmogon.Data.Items;
using Schmogon.Data.Pokemon;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    private const string InsertItemQuery = "INSERT INTO Item VALUES (@name, @shortdesc);";

    private const string InsertItemToMovesetQuery =
      "INSERT INTO ItemToMoveset (Name_Item, id_Moveset) VALUES (@item, @moveset);";

    private async Task<string> insertItemData(Database database, Item item, ItemData itemData)
    {
      var statement = await database.PrepareStatementAsync(InsertItemQuery);
      statement.BindTextParameterWithName("@name", itemData.Name);
      statement.BindTextParameterWithName("@shortdesc", item.Description);

      await statement.StepAsync();

      foreach (var desc in itemData.Description)
      {
        await insertTextElement(database, desc, itemData.Name, OwnerType.Item, ElementType.Description);
      }

      foreach (var comp in itemData.Competitive)
      {
        await insertTextElement(database, comp, itemData.Name, OwnerType.Item, ElementType.Competitive);
      }

      return itemData.Name;
    }

    private async Task<long> insertMovesetItemConnections(Database database, Moveset moveset, long movesetId)
    {
      long lastKey = 0;

      foreach (var item in moveset.Items)
      {
        var statement = await database.PrepareStatementAsync(InsertItemToMovesetQuery);
        statement.BindTextParameterWithName("@item", item.Name);
        statement.BindInt64ParameterWithName("@moveset", movesetId);

        try
        {
          await statement.StepAsync();
        }
        catch (Exception)
        {
          Debugger.Break();

          continue;
        }

        lastKey = database.GetLastInsertedRowId();
      }

      return lastKey;
    }
  }
}
