using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SchmogonDB.Model.Pokemon
{
  public enum Tier
  {
    Uber,
    OU,
    BL,
    UU,
    BL2,
    RU,
    NU,
    LC,
    Limbo,
    NFE
  }

  public static class TierUtils
  {
    private static readonly IReadOnlyDictionary<Tier, string> TierNames 
      = new ReadOnlyDictionary<Tier, string>
    (
      new Dictionary<Tier, string>
      {
        {Tier.Uber, "Uber"},
        {Tier.OU, "Overused"},
        {Tier.BL, "Borderline"},
        {Tier.UU, "Underused"},
        {Tier.BL2, "Borderline 2"},
        {Tier.RU, "Rarely Used"},
        {Tier.NU, "Never Used"},
        {Tier.LC, "Little Cup"},
        {Tier.Limbo, "Limbo"},
        {Tier.NFE, "Not Fully Evolved"}
      }
    );

    public static string GetName(Tier tier)
    {
      return TierNames[tier];
    }
  }
}
