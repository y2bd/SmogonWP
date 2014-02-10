using SchmogonDB.Population;
using SQLiteWinRT;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SchmogonDB
{
  public partial class SchmogonDBClient : ISchmogonDBClient
  {
    private static readonly StorageFolder DbRootPath = ApplicationData.Current.LocalFolder;
    private const string DbName = "pokemon.db";

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
      if (_isInitiatlized) return;

      await _database.OpenAsync();
      await ensureForeignKeys();

      await _populator.PopulateDatabaseAsync(_database, true, false);

      _isInitiatlized = true;
    }

    public async Task RecreateDatabase()
    {
      ensureDatabaseInitialized();

      await _populator.PopulateDatabaseAsync(_database, false, true);
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
