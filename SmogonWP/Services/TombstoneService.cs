using System;
using System.IO.IsolatedStorage;

namespace SmogonWP.Services
{
  public class TombstoneService
  {
    private IsolatedStorageSettings _store;

    public TombstoneService()
    {
      getSettingsStore();
    }

    public void Store(string key, object toSave)
    {
      if (_store.Contains(key))
      {
        _store[key] = toSave;
      }
      else
      {
        _store.Add(key, toSave);
      }
    }

    public void Save()
    {
      _store.Save();
    }

    public T Load<T>(string key)
    {
      T result;

      if (!_store.TryGetValue(key, out result))
      {
        result = default(T);
      }

      return result;
    }

    private void getSettingsStore()
    {
      try
      {
        _store = IsolatedStorageSettings.ApplicationSettings;
      }
      catch (Exception e)
      {
        throw new Exception("Failed to access ApplicationSettings.", e);
      }
    }
  }
}
