namespace SchmogonDB.Model.Stats
{
  public struct BaseStat
  {
    public int HP { get; set; }

    public int Attack { get; set; }

    public int Defense { get; set; }

    public int SpecialAttack { get; set; }

    public int SpecialDefense { get; set; }

    public int Speed { get; set; }

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