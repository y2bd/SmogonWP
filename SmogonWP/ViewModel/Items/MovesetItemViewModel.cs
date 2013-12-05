using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Schmogon.Data.Natures;
using Schmogon.Data.Pokemon;
using Schmogon.Model.Text;
using SchmogonDB.Model;
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
    public IEnumerable<TypedGroupMoveItemViewModel> Moves { get; private set; }

    public MovesetItemViewModel(string ownerName, Moveset data)
    {
      OwnerName = ownerName;
      Data = data;

      Abilities = data.Abilities == null ? 
        new List<AbilityItemViewModel>() 
        : data.Abilities.Select(a => new AbilityItemViewModel(a)).ToList();

      Natures = data.Natures == null 
        ? new List<string>() 
        : data.Natures.Select(n => Enum.GetName(typeof(Nature), n)).ToList();

      Items = data.Items == null
        ? new List<ItemItemViewModel>()
        : data.Items.Select(i => new ItemItemViewModel(i)).ToList();

      Moves = data.Moves == null
        ? new List<TypedGroupMoveItemViewModel>()
        : data.Moves.SelectMany((mg, i) => mg.Select(m => new TypedGroupMoveItemViewModel(m as , i)));

      // TODO: Write the DataTemplates in the View that can display this new type of list

    }
  }
}
