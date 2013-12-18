using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using SchmogonDB.Model.Pokemon;
using SmogonWP.Utilities;

namespace SmogonWP.ViewModel.Items
{
  public class PokemonDataItemViewModel : ViewModelBase
  {
    private const string SmogonBase = "http://smogon.com";

    public PokemonData Data { get; private set; }

    public string Name
    {
      get { return Data.Name.ToUpperInvariant(); }
    }

    public string Tier
    {
      get { return TierUtils.GetName(Data.Tier).ToLower(); }
    }

    public IEnumerable<AbilityItemViewModel> Abilities { get; private set; }
    public IEnumerable<TypeItemViewModel> Types { get; private set; }
    public IEnumerable<MoveItemViewModel> Moves { get; private set; }

    private ObservableCollection<MovesetItemViewModel> _movesets;
    public ObservableCollection<MovesetItemViewModel> Movesets
    {
      get
      {
        return _movesets;
      }
      set
      {
        if (_movesets != value)
        {
          _movesets = value;
          RaisePropertyChanged(() => Movesets);
        }
      }
    }

    private string _spritePath;
    public string SpritePath
    {
      get
      {
        return _spritePath;
      }
      set
      {
        if (_spritePath != value)
        {
          _spritePath = value;
          RaisePropertyChanged(() => SpritePath);
        }
      }
    }			

    public PokemonDataItemViewModel(PokemonData data)
    {
      Data = data;

      Abilities = Data.Abilities.OrderByDescending(a => TextUtils.EstimateTextLength(a.Name)).Select((a, i) => new AbilityItemViewModel(a, i)).ToList();
      Types = Data.Types.Select(t => new TypeItemViewModel(t)).ToList();
      Moves = Data.Moves.Select(m => new MoveItemViewModel(m)).ToList();

      SpritePath = SmogonBase + Data.SpritePath;

      _movesets = new ObservableCollection<MovesetItemViewModel>(Data.Movesets.Select(m => new MovesetItemViewModel(Data.Name, m)));
    }
  }
}
