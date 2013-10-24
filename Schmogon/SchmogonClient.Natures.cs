using System.Collections.Generic;
using System.Linq;
using Schmogon.Data.Natures;
using Schmogon.Data.Stats;

namespace Schmogon
{
  public partial class SchmogonClient
  {
    public IEnumerable<NatureEffect> GetAllNatureEffects()
    {
      return NatureEffect.NatureEffects;
    }

    public NatureEffect GetNatureEffect(Nature nature)
    {
      return NatureEffect.NatureEffects.First(n => n.Nature == nature);
    }

    public IEnumerable<NatureEffect> GetNatureEffectWhere(StatType increased, StatType decreased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Increased == increased &&
                                              n.Decreased == decreased &&
                                              !n.IsNeutral);
    }

    public IEnumerable<NatureEffect> GetNatureEffectWhereIncreased(StatType increased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Increased == increased &&
                                              !n.IsNeutral);
    }

    public IEnumerable<NatureEffect> GetNatureEffectWhereDecreased(StatType decreased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Decreased == decreased &&
                                              !n.IsNeutral);
    }

  }
}
