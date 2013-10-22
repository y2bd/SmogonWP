using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace SmogonWP.Services
{
  public class TrayService : ObservableObject
  {
    private readonly List<Tuple<string, string>> _jobs;
    private readonly List<Tuple<string, int>> _progresses;

    private bool _isLoading;
    public bool IsLoading
    {
      get { return _isLoading; }
      private set
      {
        if (value != _isLoading)
        {
          _isLoading = value;
          RaisePropertyChanged(() => IsLoading);
        }
      }
    }

    public string CurrentMessage
    {
      get {
        return _jobs.Count > 0 ? _jobs.Last().Item2 : string.Empty;
      }
    }

    public TrayService()
    {
      _jobs = new List<Tuple<string, string>>();
      _progresses = new List<Tuple<string, int>>();
    }

    public void AddJob(string key, string message)
    {
      // don't add duplicate jobs you fool!
      if (_jobs.Any(t => t.Item1.Equals(key))) return;

      _jobs.Add(new Tuple<string, string>(key, message));

      if (!IsLoading) IsLoading = true;

      RaisePropertyChanged(() => CurrentMessage);
    }

    public void RemoveJob(string key)
    {
      _jobs.RemoveAll(t => t.Item1.Equals(key));

      if (_jobs.Count <= 0)
      {
        IsLoading = false;
      }

      RaisePropertyChanged(() => CurrentMessage);
    }

    public void RemoveAllJobs()
    {
      _jobs.Clear();

      IsLoading = false;

      RaisePropertyChanged(() => CurrentMessage);
    }

  }
}
