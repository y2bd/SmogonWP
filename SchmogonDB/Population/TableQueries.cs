namespace SchmogonDB.Population
{
  internal partial class Populator
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
        Name TEXT NOT NULL  PRIMARY KEY,
        ShortDescription TEXT
        );";

    private const string CreateItemTableQuery =
      @"CREATE TABLE IF NOT EXISTS Item (
        Name TEXT NOT NULL  PRIMARY KEY,
        ShortDescription TEXT
        );";

    private const string CreateMoveTableQuery =
      @"CREATE TABLE IF NOT EXISTS Move (
        Name TEXT NOT NULL  PRIMARY KEY,
        ShortDescription TEXT,
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
        RelationDescription TEXT,
        Name_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name),
        Name_Move TEXT NOT NULL  REFERENCES Move (Name)
        );";

    private const string CreateMoveToMoveTableQuery =
      @"CREATE TABLE IF NOT EXISTS MoveToMove (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        RelationDescription TEXT,
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

    private const string CreateTeamTableQuery =
      @"CREATE TABLE IF NOT EXISTS Team (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        Type INTEGER,
        Name TEXT 
        );";

    private const string CreateTeamMemberTableQuery =
      @"CREATE TABLE IF NOT EXISTS TeamMember (
        id INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,
        id_Team INTEGER NOT NULL  REFERENCES Team (id),
        id_Pokemon TEXT NOT NULL  REFERENCES Pokemon (Name),
        id_Move1 TEXT NOT NULL  REFERENCES Move (Name),
        id_Move2 TEXT NOT NULL  REFERENCES Move (Name),
        id_Move3 TEXT NOT NULL  REFERENCES Move (Name),
        id_Move4 TEXT NOT NULL  REFERENCES Move (Name),
        id_Ability TEXT NOT NULL  REFERENCES Ability (Name),
        id_Item TEXT NOT NULL  REFERENCES Item (Name),
        Nature INTEGER,
        Level INTEGER,
        EV_HP INTEGER,
        EV_Attack INTEGER,
        EV_Defense INTEGER,
        EV_SpecialAttack INTEGER,
        EV_SpecialDefense INTEGER,
        EV_Speed INTEGER,
        IV_HP INTEGER,
        IV_Attack INTEGER,
        IV_Defense INTEGER,
        IV_SpecialAttack INTEGER,
        IV_SpecialDefense INTEGER,
        IV_Speed INTEGER
        );";

    #endregion level 4

    private const string CreateIndicesQuery =
      @"CREATE INDEX IF NOT EXISTS [atm_foreign] ON [AbilityToMoveset](
        [Name_Ability]  ASC,
        [id_Moveset]  ASC
        );

        CREATE INDEX IF NOT EXISTS [atp_foreign] ON [AbilityToPokemon](
        [Name_Ability]  ASC,
        [Name_Pokemon]  ASC
        );

        CREATE INDEX IF NOT EXISTS [itm_foreign] ON [ItemToMoveset](
        [Name_Item]  ASC,
        [id_Moveset]  ASC
        );

        CREATE INDEX IF NOT EXISTS [mc_foreign] ON [MoveCollection](
        [id_Moveset]  ASC
        );

        CREATE INDEX IF NOT EXISTS [mtm_foreign] ON [MoveToMove](
        [Name_MoveFrom]  ASC,
        [Name_MoveTo]  ASC
        );

        CREATE INDEX IF NOT EXISTS [mtmc_foreign] ON [MoveToMoveCollection](
        [Name_Move]  ASC,
        [id_MoveCollection]  ASC
        );

        CREATE INDEX IF NOT EXISTS [mtp_foreign] ON [MoveToPokemon](
        [Name_Pokemon]  ASC,
        [Name_Move]  ASC
        );

        CREATE INDEX IF NOT EXISTS [moveset_foreign] ON [Moveset](
        [Name_Pokemon]  ASC
        );

        CREATE INDEX IF NOT EXISTS [mn_foreign] ON [MovesetNature](
        [id_Moveset]  ASC
        );

        CREATE INDEX IF NOT EXISTS [pt_foreign] ON [PokemonType](
        [Name_Pokemon]  ASC
        );

        CREATE INDEX IF NOT EXISTS [te_owner_id] ON [TextElement](
        [OwnerId]  ASC
        );

        CREATE INDEX IF NOT EXISTS [tec_foreign] ON [TextElementContent](
        [id_TextElement]  ASC
        );";
  }
}
