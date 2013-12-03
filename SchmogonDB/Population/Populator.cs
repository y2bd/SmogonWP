using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SQLiteWinRT;

namespace SchmogonDB.Population
{
  internal partial class Populator
  {
    public async Task PopulateDatabaseAsync(Database database, bool justCreateTables=false, bool dropTablesFirst=false)
    {
      if (dropTablesFirst) await dropAllTables(database);

      await createTables(database);
      await createIndices(database);

      if (justCreateTables) return;

      await fillTables(database);
    }

    private async Task createTables(Database database)
    {
      await createLevel1Tables(database);
      await createLevel2Tables(database);
      await createLevel3Tables(database);
      await createLevel4Tables(database);
    }

    private async Task createLevel1Tables(Database database)
    {
      await database.ExecuteStatementAsync(CreatePokemonTableQuery);
      await database.ExecuteStatementAsync(CreateAbilityTableQuery);
      await database.ExecuteStatementAsync(CreateItemTableQuery);
      await database.ExecuteStatementAsync(CreateMoveTableQuery);
      await database.ExecuteStatementAsync(CreateTextElementTableQuery);
    }

    private async Task createLevel2Tables(Database database)
    {
      await database.ExecuteStatementAsync(CreateAbilityToPokemonTableQuery);
      await database.ExecuteStatementAsync(CreatePokemonTypeTableQuery);
      await database.ExecuteStatementAsync(CreateMoveToPokemonTableQuery);
      await database.ExecuteStatementAsync(CreateMoveToMoveTableQuery);
      await database.ExecuteStatementAsync(CreateMovesetTableQuery);
      await database.ExecuteStatementAsync(CreateTextElementContentTableQuery);
    }

    private async Task createLevel3Tables(Database database)
    {
      await database.ExecuteStatementAsync(CreateItemToMovesetTableQuery);
      await database.ExecuteStatementAsync(CreateAbilityToMovesetTableQuery);
      await database.ExecuteStatementAsync(CreateMoveCollectionTableQuery);
      await database.ExecuteStatementAsync(CreateMovesetNatureTableQuery);
    }

    private async Task createLevel4Tables(Database database)
    {
      await database.ExecuteStatementAsync(CreateMoveToMoveCollectionTableQuery);
    }

    private async Task createIndices(Database database)
    {
      await database.ExecuteStatementAsync(CreateIndicesQuery);
    }

    private async Task dropAllTables(Database database)
    {
      const string query =
        @"PRAGMA writable_schema = 1;
          delete from sqlite_master where type in ('table', 'index');
          PRAGMA writable_schema = 0;
          VACUUM;
          PRAGMA INTEGRITY_CHECK;";

      await database.ExecuteStatementAsync(query);
    }

    private async Task fillTables(Database database)
    {
      var loader = new Loader();

      var data = await loader.LoadAllData();

      foreach (var item in data.Items)
      {
        var itemSearch = data.ItemsSearch.First(i => i.Name == item.Name);

        var key = await insertItemData(database, itemSearch, item);

        Debug.WriteLine("Inserted item {0}", key);
      }

      foreach (var ability in data.Abilities)
      {
        var abilSearch = data.AbilitiesSearch.First(a => a.Name == ability.Name);

        var key = await insertAbilityData(database, abilSearch, ability);

        Debug.WriteLine("Inserted ability {0}", key);
      }

      foreach (var move in data.Moves)
      {
        var moveSearch = data.MovesSearch.First(m => m.Name == move.Name);

        var key = await insertMoveData(database, moveSearch, move);

        Debug.WriteLine("Inserted {0}", key);
      }

      foreach (var move in data.Moves)
      {
        try
        {
          await insertRelatedMoveConnections(database, move);
        }
        catch (Exception)
        {
          Debug.WriteLine("Failed related for {0}", move.Name);
          Debugger.Break();

          continue;
        }

        Debug.WriteLine("Inserted related move connections for {0}", move.Name);
      }

      foreach (var pokemon in data.Pokemon)
      {
        try
        {
          await insertPokemonData(database, pokemon);
        }
        catch (Exception)
        {
          Debug.WriteLine("Failed for {0}", pokemon.Name);
          Debugger.Break();

          continue;
        }

        Debug.WriteLine("Inserted Pokemon {0}", pokemon.Name);
      }
    }
  }
}
