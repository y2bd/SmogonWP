using System.Diagnostics;
using SQLiteWinRT;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private static readonly StorageFolder DbRootPath = ApplicationData.Current.LocalFolder;
    private const string DbName = "pokemon.db";

    private Database _database;

    public async Task CreateDatabase(bool dropTablesFirst = false)
    {
      if (_database != null) return;

      _database = new Database(DbRootPath, DbName);
      await _database.OpenAsync();
      await ensureForeignKeys();

      if (dropTablesFirst) await dropAllTables();

      await createTables();
      await createIndices();
    }

    public async Task PopulateTables()
    {
      var loader = new Loader();

      var data = await loader.LoadAllData();

      foreach (var item in data.Items)
      {
        var key = await insertItemData(item);

        Debug.WriteLine("Inserted item {0}", key);
      }

      foreach (var ability in data.Abilities)
      {
        var key = await insertAbilityData(ability);

        Debug.WriteLine("Inserted ability {0}", key);
      }

      foreach (var move in data.Moves)
      {
        var key = await insertMoveData(move);

        Debug.WriteLine("Inserted {0}", key);
      }

      foreach (var move in data.Moves)
      {
        try
        {
          await insertRelatedMoveConnections(move);
        }
        catch (Exception e)
        {
          Debug.WriteLine("Failed related for {0}", move.Name);

          continue;
        }

        Debug.WriteLine("Inserted related move connections for {0}", move.Name);
      }

      foreach (var pokemon in data.Pokemon)
      {
        try
        {
          await insertPokemonData(pokemon);
        }
        catch (Exception e)
        {
          Debug.WriteLine("Failed for {0}", pokemon.Name);

          continue;
        }

        Debug.WriteLine("Inserted Pokemon {0}", pokemon.Name);
      }
    }

    private async Task ensureForeignKeys()
    {
      await _database.ExecuteStatementAsync("PRAGMA foreign_keys = ON;");
    }

    private async Task createTables()
    {
      await createLevel1Tables();
      await createLevel2Tables();
      await createLevel3Tables();
      await createLevel4Tables();
    }

    private async Task createLevel1Tables()
    {
      await _database.ExecuteStatementAsync(CreatePokemonTableQuery);
      await _database.ExecuteStatementAsync(CreateAbilityTableQuery);
      await _database.ExecuteStatementAsync(CreateItemTableQuery);
      await _database.ExecuteStatementAsync(CreateMoveTableQuery);
      await _database.ExecuteStatementAsync(CreateTextElementTableQuery);
    }

    private async Task createLevel2Tables()
    {
      await _database.ExecuteStatementAsync(CreateAbilityToPokemonTableQuery);
      await _database.ExecuteStatementAsync(CreatePokemonTypeTableQuery);
      await _database.ExecuteStatementAsync(CreateMoveToPokemonTableQuery);
      await _database.ExecuteStatementAsync(CreateMoveToMoveTableQuery);
      await _database.ExecuteStatementAsync(CreateMovesetTableQuery);
      await _database.ExecuteStatementAsync(CreateTextElementContentTableQuery);
    }

    private async Task createLevel3Tables()
    {
      await _database.ExecuteStatementAsync(CreateItemToMovesetTableQuery);
      await _database.ExecuteStatementAsync(CreateAbilityToMovesetTableQuery);
      await _database.ExecuteStatementAsync(CreateMoveCollectionTableQuery);
      await _database.ExecuteStatementAsync(CreateMovesetNatureTableQuery);
    }

    private async Task createLevel4Tables()
    {
      await _database.ExecuteStatementAsync(CreateMoveToMoveCollectionTableQuery);
    }

    private async Task createIndices()
    {
      await _database.ExecuteStatementAsync(CreateIndicesQuery);
    }

    private async Task dropAllTables()
    {
      const string query = 
        @"PRAGMA writable_schema = 1;
          delete from sqlite_master where type = 'table';
          PRAGMA writable_schema = 0;
          VACUUM;
          PRAGMA INTEGRITY_CHECK;";

      await _database.ExecuteStatementAsync(query);
    }
  }
}
