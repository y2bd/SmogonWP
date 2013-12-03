using SchmogonDB.Population;
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

    private readonly Database _database;

    private readonly Populator _populator;

    public SchmogonDBClient()
    {
      _database = new Database(DbRootPath, DbName);

      _populator = new Populator();
    }

    public async Task InitializeDatabase(bool dropTablesFirst = false)
    {
      await _database.OpenAsync();
      await ensureForeignKeys();

      await _populator.PopulateDatabaseAsync(_database, dropTablesFirst);
    }
    
    private async Task ensureForeignKeys()
    {
      await _database.ExecuteStatementAsync("PRAGMA foreign_keys = ON;");
    }
  }
}
