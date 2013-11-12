using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SmogonWP.ViewModel.Items;

namespace SmogonWP.Model
{
  /// <remarks>
  /// This is kind of a hack, seeing as there will never be more than one Moveset per group. This is just to get the LLS to behave how we want it to.
  /// </remarks>
  public class MovesetGroup : ObservableCollection<MovesetItemViewModel>, INotifyPropertyChanged
  {
    private string _name;
    public string Name
    {
      get
      {
        return _name.ToUpperInvariant();
      }
      set
      {
        if (_name != value)
        {
          _name = value;

          NotifyPropertyChanged();
        }
      }
    }

    public MovesetGroup(string name)
    {
      _name = name;
    }

    public MovesetGroup(IEnumerable<MovesetItemViewModel> collection, string name)
      : base(collection)
    {
      _name = name;
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
