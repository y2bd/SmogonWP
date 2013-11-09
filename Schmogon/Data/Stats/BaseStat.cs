using Newtonsoft.Json;

namespace Schmogon.Data.Stats
{
  public struct BaseStat
  {
    [JsonProperty("HP")]
    public int HP { get; private set; }

    [JsonProperty("Attack")]
    public int Attack { get; private set; }

    [JsonProperty("Defense")]
    public int Defense { get; private set; }

    [JsonProperty("SpecialAttack")]
    public int SpecialAttack { get; private set; }

    [JsonProperty("SpecialDefense")]
    public int SpecialDefense { get; private set; }

    [JsonProperty("Speed")]
    public int Speed { get; private set; }

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
  }
}