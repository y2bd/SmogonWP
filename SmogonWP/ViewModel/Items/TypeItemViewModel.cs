using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Type = Schmogon.Data.Types.Type;

namespace SmogonWP.ViewModel.Items
{
  public class TypeItemViewModel : ViewModelBase
  {
    public static readonly IReadOnlyDictionary<Type, Color> TypeColors = new ReadOnlyDictionary<Type, Color>(
      new Dictionary<Type, Color>
      {
        {Type.Normal, Color.FromArgb(255, 205, 133, 63)},
        {Type.Fire, Color.FromArgb(255, 255, 69, 0)},
        {Type.Water, Color.FromArgb(255, 30, 144, 255)},
        {Type.Electric, Color.FromArgb(255, 235, 200, 0)},
        {Type.Grass, Color.FromArgb(255, 0, 100, 0)},
        {Type.Ice, Color.FromArgb(255, 72, 209, 204)},
        {Type.Fighting, Color.FromArgb(255, 165, 42, 42)},
        {Type.Poison, Color.FromArgb(255, 153, 50, 204)},
        {Type.Ground, Color.FromArgb(255, 244, 164, 96)},
        {Type.Flying, Color.FromArgb(255, 123, 103, 238)},
        {Type.Psychic, Color.FromArgb(255, 186, 85, 211)},
        {Type.Bug, Color.FromArgb(255, 154, 205, 50)},
        {Type.Rock, Color.FromArgb(255, 160, 82, 45)},
        {Type.Ghost, Color.FromArgb(255, 106, 90, 205)},
        {Type.Dragon, Color.FromArgb(255, 65, 105, 225)},
        {Type.Dark, Color.FromArgb(255, 35, 32, 38)},
        {Type.Steel, Color.FromArgb(255, 112, 128, 144)},
        {Type.Fairy, Color.FromArgb(255, 218, 112, 214)}
      }
    );

    internal readonly Type Type;
    
    private string _name;
    public string Name
    {
      get
      {
        return _name.ToLowerInvariant();
      }
      set
      {
        if (_name != value)
        {
          _name = value;
          RaisePropertyChanged(() => Name);
        }
      }
    }

    private SolidColorBrush _backgroundBrush;
    public SolidColorBrush BackgroundBrush
    {
      get
      {
        return _backgroundBrush;
      }
      set
      {
        if (_backgroundBrush != value)
        {
          _backgroundBrush = value;
          RaisePropertyChanged(() => BackgroundBrush);
        }
      }
    }			

    public TypeItemViewModel(Type type)
    {
      Type = type;

      Name = Enum.GetName(typeof(Type), type);

      BackgroundBrush = new SolidColorBrush(TypeColors[type]);
    }
  }
}
