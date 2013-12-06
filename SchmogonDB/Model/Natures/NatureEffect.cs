using System.Collections.Generic;
using SchmogonDB.Model.Stats;

namespace SchmogonDB.Model.Natures
{
  public class NatureEffect
  {
    private static readonly IEnumerable<NatureEffect> _natureEffects = new List<NatureEffect>
    {
      new NatureEffect(Nature.Adamant, StatType.Attack, StatType.SpecialAttack),
      new NatureEffect(Nature.Bashful, StatType.SpecialAttack, StatType.SpecialAttack),
      new NatureEffect(Nature.Bold, StatType.Defense, StatType.Attack),
      new NatureEffect(Nature.Brave, StatType.Attack, StatType.Speed),
      new NatureEffect(Nature.Calm, StatType.SpecialDefense, StatType.Attack),
      new NatureEffect(Nature.Careful, StatType.SpecialDefense, StatType.SpecialAttack),
      new NatureEffect(Nature.Docile, StatType.Defense, StatType.Defense),
      new NatureEffect(Nature.Gentle, StatType.SpecialDefense, StatType.Defense),
      new NatureEffect(Nature.Hardy, StatType.Attack, StatType.Attack),
      new NatureEffect(Nature.Hasty, StatType.Speed, StatType.Defense),
      new NatureEffect(Nature.Impish, StatType.Defense, StatType.SpecialAttack),
      new NatureEffect(Nature.Jolly, StatType.Speed, StatType.SpecialAttack),
      new NatureEffect(Nature.Lax, StatType.Defense, StatType.SpecialDefense),
      new NatureEffect(Nature.Lonely, StatType.Attack, StatType.Defense),
      new NatureEffect(Nature.Mild, StatType.SpecialAttack, StatType.Defense),
      new NatureEffect(Nature.Modest, StatType.SpecialAttack, StatType.Attack),
      new NatureEffect(Nature.Naive, StatType.Speed, StatType.SpecialDefense),
      new NatureEffect(Nature.Naughty, StatType.Attack, StatType.SpecialDefense),
      new NatureEffect(Nature.Quiet, StatType.SpecialAttack, StatType.Speed),
      new NatureEffect(Nature.Quirky, StatType.SpecialDefense, StatType.SpecialDefense),
      new NatureEffect(Nature.Rash, StatType.SpecialAttack, StatType.SpecialDefense),
      new NatureEffect(Nature.Relaxed, StatType.Defense, StatType.Speed),
      new NatureEffect(Nature.Sassy, StatType.SpecialDefense, StatType.Speed),
      new NatureEffect(Nature.Serious, StatType.Speed, StatType.Speed),
      new NatureEffect(Nature.Timid, StatType.Speed, StatType.Attack),
    };

    internal static IEnumerable<NatureEffect> NatureEffects
    {
      get { return _natureEffects; }
    }

    public Nature Nature { get; private set; }
    public StatType Increased { get; private set; }
    public StatType Decreased { get; private set; }

    public bool IsNeutral
    {
      get { return Increased == Decreased; }
    }

    private NatureEffect(Nature nature, StatType increased, StatType decreased)
    {
      Nature = nature;
      Increased = increased;
      Decreased = decreased;
    }
  }
}
