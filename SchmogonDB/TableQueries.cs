namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    /* I've organized the various table queries into levels.
     * The levels show in which order they should be created
     * because the further you go, the more relationships are
     * established, requireing other tables to exist
     */

    #region level 1
    private const string CreatePokemonTableQuery =
      @"CREATE TABLE IF NOT EXISTS Pokemon (
        Name TEXT NOT NULL  PRIMARY KEY,
        SpritePath TEXT,
        Tier INTEGER,
        HP INTEGER,
        Attack INTEGER,
        Defense INTEGER,
        SpecialAttack INTEGER,
        SpecialDefense INTEGER,
        Speed INTEGER
        );";

    private const string CreateAbilityTableQuery =
      @"CREATE TABLE IF NOT EXISTS Ability (
        Name TEXT NOT NULL  PRIMARY KEY
        );";

    private const string CreateItemTableQuery =
      @"CREATE TABLE IF NOT EXISTS Item (
        Name TEXT NOT NULL  PRIMARY KEY
        );";

    private const string CreateMoveTableQuery =
      @"CREATE TABLE IF NOT EXISTS Move (
        Name TEXT NOT NULL  PRIMARY KEY,
        Type INTEGER,
        Power TEXT,
        Accuracy TEXT,
        PP TEXT,
        Priority TEXT,
        Damage TEXT,
        Target TEXT
        );";

    private const string CreateTextElementTableQuery =
      @"CREATE TABLE IF NOT EXISTS TextElement (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        OwnerId TEXT,
        OwnerType INTEGER,
        ElementType INTEGER
        );";
    #endregion level 1

    #region level 2

    private const string CreateAbilityToPokemonTableQuery =
      @"CREATE TABLE IF NOT EXISTS AbilityToPokemon (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_Ability TEXT NOT NULL  REFERENCES Ability (Name),
        Name_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name)
        );";

    private const string CreatePokemonTypeTableQuery =
      @"CREATE TABLE IF NOT EXISTS PokemonType (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Type INTEGER,
        Name_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name)
        );";

    private const string CreateMoveToPokemonTableQuery =
      @"CREATE TABLE IF NOT EXISTS MoveToPokemon (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name),
        Name_Move TEXT NOT NULL  REFERENCES Move (Name)
        );";

    private const string CreateMoveToMoveTableQuery =
      @"CREATE TABLE IF NOT EXISTS MoveToMove (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_MoveFrom TEXT NOT NULL  REFERENCES Move (Name),
        Name_MoveTo TEXT NOT NULL  REFERENCES Move (Name),
        MoveTo_FullName TEXT
        );";

    private const string CreateMovesetTableQuery =
      @"CREATE TABLE IF NOT EXISTS Moveset (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name TEXT,
        EV_HP INTEGER,
        EV_Attack INTEGER,
        EV_Defense INTEGER,
        EV_SpecialAttack INTEGER,
        EV_SpecialDefense INTEGER,
        EV_Speed INTEGER,
        Name_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name)
        );";

    private const string CreateTextElementContentTableQuery =
      @"CREATE TABLE IF NOT EXISTS TextElementContent (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Content TEXT,
        id_TextElement INTEGER NOT NULL  REFERENCES TextElement (id)
        );";

    #endregion level 2

    #region level 3

    private const string CreateItemToMovesetTableQuery =
      @"CREATE TABLE IF NOT EXISTS ItemToMoveset (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_Item TEXT NOT NULL  REFERENCES Item (Name),
        id_Moveset INTEGER NOT NULL  REFERENCES Moveset (id)
        );";

    private const string CreateAbilityToMovesetTableQuery =
      @"CREATE TABLE IF NOT EXISTS AbilityToMoveset (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_Ability TEXT NOT NULL  REFERENCES Ability (Name),
        id_Moveset INTEGER NOT NULL  REFERENCES Moveset (id)
        );";

    private const string CreateMoveCollectionTableQuery =
      @"CREATE TABLE IF NOT EXISTS MoveCollection (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        id_Moveset INTEGER NOT NULL  REFERENCES Moveset (id)
        );";

    private const string CreateMovesetNatureTableQuery =
      @"CREATE TABLE IF NOT EXISTS MovesetNature (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Nature INTEGER,
        id_Moveset INTEGER NOT NULL  REFERENCES Moveset (id)
        );";

    #endregion level 3

    #region level 4

    private const string CreateMoveToMoveCollectionTableQuery =
      @"CREATE TABLE IF NOT EXISTS MoveToMoveCollection (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Name_Move TEXT NOT NULL  REFERENCES Move (Name),
        Move_FullName TEXT,
        id_MoveCollection INTEGER NOT NULL  REFERENCES MoveCollection (id)
        );";

    #endregion level 4
  }
}
