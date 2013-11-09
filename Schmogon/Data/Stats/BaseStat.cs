namespace Schmogon.Data.Stats
{
  public struct BaseStat
  {
    public int HP { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int SpecialAttack { get; private set; }
    public int SpecialDefense { get; private set; }
    public int Speed { get; private set; }

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