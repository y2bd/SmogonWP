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

    private const bool JustCreateTables = true;
    private const bool DropTablesFirst = false;

    private readonly Database _database;

    private readonly Populator _populator;

    private bool _isInitiatlized;

    public SchmogonDBClient()
    {
      _database = new Database(DbRootPath, DbName);

      _populator = new Populator();
    }

    public async Task InitializeDatabase()
    {
      await _database.OpenAsync();
      await ensureForeignKeys();

      await _populator.PopulateDatabaseAsync(_database, JustCreateTables, DropTablesFirst);

      _isInitiatlized = true;
    }
    
    private async Task ensureForeignKeys()
    {
      await _database.ExecuteStatementAsync("PRAGMA foreign_keys = ON;");
    }

    private void ensureDatabaseInitialized()
    {
      if (!_isInitiatlized)
        throw new InvalidOperationException("Database must be initialized before data can be retrieved.");
    }
  }
}
