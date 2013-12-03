using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Schmogon.Data.Items;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    private const string FetchItemSearchDataQuery =
      @"SELECT * FROM Item";

    private IEnumerable<Item> _itemCache;

    public async Task<IEnumerable<Item>> FetchItemSearchDataAsync()
    {
      ensureDatabaseInitialized();

      return _itemCache ?? (_itemCache = await fetchItemSearchData());
    }

    private async Task<IEnumerable<Item>> fetchItemSearchData()
    {
      var moves = new List<Item>();

      var statement = await _database.PrepareStatementAsync(FetchItemSearchDataQuery);

      while (statement.StepSync())
      {
        var name = statement.GetTextAt(0);
        var desc = statement.GetTextAt(1);

        var pageLocation = Utilities.ConstructSmogonLink(name, Utilities.ItemBasePath);

        moves.Add(new Item(name, desc, pageLocation));
      }

      return moves;
    }
  }
}
