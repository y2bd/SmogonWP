using System;
using System.Collections.Generic;
using System.Linq;
using Schmogon.Data.Types;
using Type = Schmogon.Data.Types.Type;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    private IEnumerable<TypeDefenseEffect> _defenseCache;

    public IEnumerable<TypeOffenseEffect> GetAllTypeOffenseEffects()
    {
      return TypeOffenseEffect.TypeEffects;
    }

    public IEnumerable<TypeDefenseEffect> GetAllTypeDefenseEffects()
    {
      if (_defenseCache != null) return _defenseCache;

      var off = GetAllTypeOffenseEffects();

      var types = Enum.GetValues(typeof(Type)).Cast<Type>().ToList();

      _defenseCache = (from type in types
                       let strong = off.Where(o => o.NotVeryEffectiveAgainst.Contains(type)).Select(o => o.Type)
                       let weak = off.Where(o => o.SuperEffectiveAgainst.Contains(type)).Select(o => o.Type)
                       let full = off.Where(o => o.NoEffectAgainst.Contains(type)).Select(o => o.Type)
                       select new TypeDefenseEffect(type, strong, weak, full));

      return _defenseCache;
    }

    public TypeOffenseEffect GetTypeOffenseEffect(Type type)
    {
      return GetAllTypeOffenseEffects().First(o => o.Type == type);
    }

    public TypeDefenseEffect GetTypeDefenseEffect(Type type)
    {
      return GetAllTypeDefenseEffects().First(o => o.Type == type);
    }
  }
}
