using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmogonWP.Services;

namespace SmogonWP.Design
{
  public class DesignSettingsService : ISettingsService
  {
    public void Save(string key, object value, bool deferSave = false)
    {
      return;
    }

    public T Load<T>(string key, T defaultValue = default(T))
    {
      return defaultValue;
    }

    public bool SettingRegistered(string key)
    {
      return true;
    }

    public void UnregisterSetting(string key)
    {
      return;
    }
  }
}
