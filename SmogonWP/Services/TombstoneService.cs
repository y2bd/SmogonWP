using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace SmogonWP.Services
{
  public class TombstoneService
  {
    private const string Filename = "tombstone.txt";

    private readonly AsyncLock _aLock;

    private readonly IsolatedStorageService _storageService;
    private readonly TrayService _trayService;

    private IDictionary<string, object> _store;

    public TombstoneService(IsolatedStorageService storageService, TrayService trayService)
    {
      _storageService = storageService;
      _trayService = trayService;

      _aLock = new AsyncLock();
    }

    public async Task Store(string key, object toSave, bool deferSave=false)
    {
      await LoadSettingsStoreAsync();
      
      if (_store.ContainsKey(key))
      {
        _store[key] = toSave;
      }
      else
      {
        _store.Add(key, toSave);
      }

      await Save();
    }

    public async Task Save()
    {
      await saveSettingsStore();
    }

    public async Task<T> Load<T>(string key)
    {
      await LoadSettingsStoreAsync();

      if (!_store.ContainsKey(key))
      {
        return default(T);
      }

      var result = _store[key];

      if (result == null) return default(T);

      return (T)result;
    }

    public async Task LoadSettingsStoreAsync(bool disableTrayProgress=false)
    {
      if (!disableTrayProgress) DispatcherHelper.CheckBeginInvokeOnUI(() => _trayService.AddJob("resuming", "Resuming..."));

      using (await _aLock.LockAsync())
      {
        if (_store != null)
        {
          if (!disableTrayProgress) DispatcherHelper.CheckBeginInvokeOnUI(() => _trayService.RemoveJob("resuming"));
          return;
        }

        if (await _storageService.FileExistsAsync(Filename))
        {
          var content = await _storageService.ReadStringFromFileAsync(Filename);

          var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};

          _store = await JsonConvert.DeserializeObjectAsync<IDictionary<string, object>>(content, settings);
        }
        else
        {
          _store = new Dictionary<string, object>();
        }
      }

      if (!disableTrayProgress) DispatcherHelper.CheckBeginInvokeOnUI(() => _trayService.RemoveJob("resuming"));
    }

    private async Task saveSettingsStore()
    {
      // i'd prefer an empty to a null
      if (_store == null) _store = new Dictionary<string, object>();

      var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};

      var cereal = await JsonConvert.SerializeObjectAsync(_store, Formatting.None, settings);

      await _storageService.WriteStringToFileAsync(Filename, cereal);
    }

  }
}
