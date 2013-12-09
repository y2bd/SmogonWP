using System.Collections.Generic;
using System.Linq;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;

namespace SchmogonDB.Tools
{
  public partial class SchmogonToolset
  {
    public IEnumerable<NatureEffect> GetAllNatureEffects()
    {
      return NatureEffect.NatureEffects;
    }

    public NatureEffect GetNatureEffect(Nature nature)
    {
      return NatureEffect.NatureEffects.First(n => n.Nature == nature);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhere(StatType increased, StatType decreased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Increased == increased &&
                                              n.Decreased == decreased &&
                                              !n.IsNeutral);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhereIncreased(StatType increased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Increased == increased &&
                                              !n.IsNeutral);
    }

    public IEnumerable<NatureEffect> GetNatureEffectsWhereDecreased(StatType decreased)
    {
      return NatureEffect.NatureEffects.Where(n => n.Decreased == decreased &&
                                              !n.IsNeutral);
    }
  }
}
