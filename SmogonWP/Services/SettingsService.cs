﻿using System;
using System.IO.IsolatedStorage;
using GalaSoft.MvvmLight;

namespace SmogonWP.Services
{
  public interface ISettingsService
  {
    void Save(string key, object value, bool deferSave = false);
    T Load<T>(string key, T defaultValue = default(T));
    bool SettingRegistered(string key);
    void UnregisterSetting(string key);
  }

  public class SettingsService : ISettingsService
  {
    private IsolatedStorageSettings _settings;

    public SettingsService()
    {
      getSettingsStore();
    }

    public void Save(string key, object value, bool deferSave = false)
    {
      if (SettingRegistered(key)) _settings[key] = value;
      else _settings.Add(key, value);

      if (!deferSave) _settings.Save();
    }

    public T Load<T>(string key, T defaultValue = default(T))
    {
      if (SettingRegistered(key))
      {
        return (T)_settings[key];
      }

      return defaultValue;
    }

    public bool SettingRegistered(string key)
    {
      return _settings.Contains(key);
    }

    public void UnregisterSetting(string key)
    {
      if (SettingRegistered(key)) _settings.Remove(key);
    }

    private void getSettingsStore()
    {
      try
      {
        _settings = IsolatedStorageSettings.ApplicationSettings;
      }
      catch (Exception e)
      {
        throw new Exception("Failed to access ApplicationSettings.", e);
      }
    }
  }
}
