using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Schmogon.Data.Pokemon;
using SchmogonDB.Model;
using SmogonWP.Model;

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
    public IEnumerable<TypedMoveItemViewModel> Moves { get; private set; }

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

    private ImageSource _sprite;
    public ImageSource Sprite
    {
      get
      {
        return _sprite;
      }
      set
      {
        if (_sprite != value)
        {
          _sprite = value;
          RaisePropertyChanged(() => Sprite);
        }
      }
    }

    public PokemonDataItemViewModel(PokemonData data)
    {
      Data = data;

      Abilities = Data.Abilities.Select(a => new AbilityItemViewModel(a)).OrderByDescending(a => a.Name.Length).ToList();
      Types = Data.Types.Select(t => new TypeItemViewModel(t)).ToList();
      Moves = Data.Moves.Select(m => new TypedMoveItemViewModel(m as TypedMove)).ToList();

      Sprite = new BitmapImage(new Uri(SmogonBase + Data.SpritePath));

      _movesets = new ObservableCollection<MovesetItemViewModel>(Data.Movesets.Select(m => new MovesetItemViewModel(Data.Name, m)));
    }
  }
}
