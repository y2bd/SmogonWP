using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using SchmogonDB.Model;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Text;
using SmogonWP.Utilities;

namespace SmogonWP.ViewModel.Items
{
  public class MovesetItemViewModel : ViewModelBase
  {
    public Moveset Data { get; private set; }

    public string Name
    {
      get
      {
        return Data.Name.ToLowerInvariant();
      }
    }

    private string _ownerName;
    public string OwnerName
    {
      get
      {
        return _ownerName.ToLowerInvariant();
      }
      set
      {
        if (_ownerName != value)
        {
          _ownerName = value;
          RaisePropertyChanged(() => OwnerName);
        }
      }
    }

    private string _preview;
    public string Preview
    {
      get
      {
        if (_preview == null)
        {
          var para = Data.Description.FirstOrDefault(t => t is Paragraph) as Paragraph;

          if (para != null)
          {
            _preview = para.Content;
          }
        }

        return _preview;
      }
    }

    public IEnumerable<AbilityItemViewModel> Abilities { get; private set; }
    public IEnumerable<string> Natures { get; private set; }
    public IEnumerable<ItemItemViewModel> Items { get; private set; }
    public IEnumerable<GroupedMoveItemViewModel> Moves { get; private set; }

    public MovesetItemViewModel(string ownerName, Moveset data)
    {
      OwnerName = ownerName;
      Data = data;

      Abilities = data.Abilities == null ? 
        new List<AbilityItemViewModel>() 
        : data.Abilities.Select((a, i) => new AbilityItemViewModel(a, i)).ToList();

      Natures = data.Natures == null 
        ? new List<string>()
        : data.Natures.Select((n, i) => Enum.GetName(typeof(Nature), n)).ToList();

      Items = data.Items == null
        ? new List<ItemItemViewModel>()
        : data.Items.Select((i, ind) => new ItemItemViewModel(i, ind)).ToList();

      Moves = data.Moves == null
        ? new List<GroupedMoveItemViewModel>()
        : data.Moves.Select((mg, gi) => new {mg, gi}).SelectMany(a => a.mg.Select(m => new {m, a.gi})).Select((a, i) => new GroupedMoveItemViewModel(a.m, a.gi, i));

      // TODO: Write the DataTemplates in the View that can display this new type of list

    }
  }
}
