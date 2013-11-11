using Newtonsoft.Json;

namespace Schmogon.Data.Stats
{
  public struct BaseStat
  {
    [JsonProperty("HP")]
    public int HP { get; internal set; }

    [JsonProperty("Attack")]
    public int Attack { get; internal set; }

    [JsonProperty("Defense")]
    public int Defense { get; internal set; }

    [JsonProperty("SpecialAttack")]
    public int SpecialAttack { get; internal set; }

    [JsonProperty("SpecialDefense")]
    public int SpecialDefense { get; internal set; }

    [JsonProperty("Speed")]
    public int Speed { get; internal set; }

    [JsonIgnore]
    public int BaseStatTotal
    {
      get
      {
        return HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;
      }
    }

    public BaseStat(int hp, int attack, int defense, int specialAttack, int specialDefense, int speed) : this()
    {
      HP = hp;
      Attack = attack;
      Defense = defense;
      SpecialAttack = specialAttack;
      SpecialDefense = specialDefense;
      Speed = speed;
    }

    public override string ToString()
    {
      return string.Format("{0} / {1} / {2} / {3} / {4} / {5}", 
        HP, Attack, Defense, SpecialAttack, SpecialDefense, Speed);
    }
  }
}