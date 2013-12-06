using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SchmogonDB.Model.Types;
using Type = SchmogonDB.Model.Types.Type;

namespace SchmogonDB.Tools
{
  public partial class SchmogonToolset
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

    public DualTypeDefenseEffect GetTypeDefenseEffect(Type type1, Type type2)
    {
      // this is tricky
      // we're essentially iterating over a cross join
      // performance matters for once

      var usedTypes = new HashSet<Type>();

      var type1Def = GetTypeDefenseEffect(type1);
      var type2Def = GetTypeDefenseEffect(type2);

      var weak1 = new HashSet<Type>(type2Def.WeakDefenseAgainst);
      var strong1 = new HashSet<Type>(type2Def.StrongDefenseAgainst);

      var weak2 = new HashSet<Type>(type2Def.WeakDefenseAgainst);
      var strong2 = new HashSet<Type>(type2Def.StrongDefenseAgainst);

      var veryWeak = new List<Type>();
      var weak = new List<Type>();
      var strong = new List<Type>();
      var veryStrong = new List<Type>();
      var full = new List<Type>();

      foreach (var f1 in type1Def.FullDefenseAgainst)
      {
        full.Add(f1);

        usedTypes.Add(f1);
      }

      foreach (var f2 in type2Def.FullDefenseAgainst)
      {
        if (usedTypes.Contains(f2)) continue;

        full.Add(f2);

        usedTypes.Add(f2);
      }

      foreach (var w1 in type1Def.WeakDefenseAgainst)
      {
        if (usedTypes.Contains(w1)) continue;

        if (weak2.Contains(w1))
        {
          // both types are weak to w1, the pairing is very weak
          veryWeak.Add(w1);
        }
        else if (strong2.Contains(w1))
        {
          // one is strong, one is weak, they cancel each other out
        }
        else
        {
          // we don't need to check for full defense
          // because we already cycled through all of those

          // it's just normal weakness
          weak.Add(w1);
        }

        usedTypes.Add(w1);
      }

      foreach (var w2 in type2Def.WeakDefenseAgainst)
      {
        if (usedTypes.Contains(w2)) continue;

        if (weak1.Contains(w2))
        {
          veryWeak.Add(w2);
        }
        else if (strong1.Contains(w2))
        {
        }
        else
        {
          weak.Add(w2);
        }

        usedTypes.Add(w2);
      }

      foreach (var s1 in type1Def.StrongDefenseAgainst)
      {
        if (usedTypes.Contains(s1)) continue;

        if (strong2.Contains(s1))
        {
          // both are strong, pairing is very strong
          veryStrong.Add(s1);
        }
        else
        {
          // we don't need to check for strong/weak or strong/complete
          // because we already cycled through all of the weak/complete types

          strong.Add(s1);
        }

        usedTypes.Add(s1);
      }

      foreach (var s2 in type2Def.StrongDefenseAgainst)
      {
        if (usedTypes.Contains(s2)) continue;

        if (strong1.Contains(s2))
        {
          veryStrong.Add(s2);
        }
        else
        {
          strong.Add(s2);
        }
      }

      return new DualTypeDefenseEffect(type1, type2, strong, weak, full, veryStrong, veryWeak);
    }
  }
}
