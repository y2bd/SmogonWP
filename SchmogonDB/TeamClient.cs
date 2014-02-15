using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Teams;

namespace SchmogonDB
{
  public partial class SchmogonDBClient
  {
    #region queries

    private const string InsertTeamQuery =
      @"INSERT INTO Team (Type, Name)
        VALUES (@type, @name);";

    private const string InsertTeamMemberQuery =
      @"INSERT INTO TeamMember (id_Team, id_Pokemon, id_Move1, id_Move2, id_Move3, id_Move4, id_Ability, id_Item, Nature, Level, EV_HP, EV_Attack, EV_Defense, EV_SpecialAttack, EV_SpecialDefense, EV_Speed, IV_HP, IV_Attack, IV_Defense, IV_SpecialAttack, IV_SpecialDefense, IV_Speed)
        VALUES (@teamid, @pokemon, @move1, @move2, @move3, @move4, @ability, @item, @nature, @level, @ehp, @eatk, @edef, @espa, @espd, @espe, @ihp, @iatk, @idef, @ispa, @ispd, @ispe);";

    private const string FetchAllTeamsQuery =
      @"SELECT t.id as TeamId,
               t.Name as Name,
               t.Type as Type,
               tm.id as MemberId,
               tm.id_Pokemon as Pokemon,
               tm.id_Move1 as Move1,
               tm.id_Move2 as Move2,
               tm.id_Move3 as Move3,
               tm.id_Move4 as Move4,
               tm.id_Ability as Ability,
               tm.id_Item as Item,
               tm.Nature,
               tm.Level,
               tm.EV_HP,
               tm.EV_Attack,
               tm.EV_Defense,
               tm.EV_SpecialAttack,
               tm.EV_SpecialDefense,
               tm.EV_Speed,
               tm.IV_HP,
               tm.IV_Attack,
               tm.IV_Defense,
               tm.IV_SpecialAttack,
               tm.IV_SpecialDefense,
               tm.IV_Speed
        FROM Team t
        LEFT JOIN TeamMember tm ON tm.id_Team = t.id";

    private const string UpdateTeamQuery =
      @"UPDATE OR IGNORE Team
        SET Type=@type,
            Name=@name
        WHERE id=@id;";

    private const string DeleteTeamQuery =
      @"DELETE FROM Team
        Where Team.id = @id;";

    private const string DeleteAllTeamMembersFromTeamQuery =
      @"DELETE FROM TeamMember
        WHERE TeamMember.id IN(
          SELECT id 
          FROM TeamMember tm
          WHERE tm.id_Team=@teamid
        );";

    private const string DeleteTeamMemberQuery =
      @"DELETE FROM TeamMember
        WHERE TeamMember.id = @id;";

    private const string UpdateTeamMemberTeamQuery =
      @"UPDATE OR IGNORE TeamMember
        SET id_Team=@teamid
        WHERE id=@id;";

    private const string UpdateTeamMemberQuery =
      @"UPDATE OR IGNORE TeamMember
        SET id_Pokemon=@pokemon,
            id_Move1=@move1,
            id_Move2=@move2,
            id_Move3=@move3,
            id_Move4=@move4,
            id_Ability=@ability,
            id_Item=@item,
            Nature=@nature,
            Level=@level,
            EV_HP=@ehp,
            EV_Attack=@eatk,
            EV_Defense=@edef,
            EV_SpecialAttack=@espa,
            EV_SpecialDefense=@espd,
            EV_Speed=@espe,
            IV_HP=@ihp,
            IV_Attack=@iatk,
            IV_Defense=@idef,
            IV_SpecialAttack=@ispa,
            IV_SpecialDefense=@ispd,
            IV_Speed=@ispe
        WHERE id=@id;";

    #endregion queries

    public async Task<IEnumerable<Team>> FetchAllTeamsAsync()
    {
      ensureDatabaseInitialized();

      var teams = new List<Team>();

      var statement = await _database.PrepareStatementAsync(FetchAllTeamsQuery);

      Team currentTeam = null;

      while (statement.StepSync())
      {
        var teamId = statement.GetIntAt(0);
        var teamName = statement.GetTextAt(1);
        var teamType = statement.GetIntAt(2);

        // if we've hit a new team block (or are just starting)
        // create a new team!
        if (currentTeam == null || currentTeam.ID != teamId)
        {
          // we were going to a new team rather than just starting
          if (currentTeam != null)
          {
            teams.Add(currentTeam);
          }

          currentTeam = new Team
          {
            ID = teamId,
            Name = teamName,
            TeamMembers = new List<TeamMember>(),
            TeamType = (TeamType) teamType
          };
        }

        var memberId = statement.GetIntAt(3);

        var pokemonName = statement.GetTextAt(4);

        if (string.IsNullOrEmpty(pokemonName))
        {
          // there's no pokemon here
          continue;
        }

        // do not be afraid! all of the following fetches are cached after the first load
        // and if they're not cached by now, then you deserve the penalty
        var pokemon = (await FetchPokemonSearchDataAsync()).First(p => p.Name == pokemonName);
        var move1 = (await FetchMoveSearchDataAsync()).First(m => m.Name == statement.GetTextAt(5));
        var move2 = (await FetchMoveSearchDataAsync()).First(m => m.Name == statement.GetTextAt(6));
        var move3 = (await FetchMoveSearchDataAsync()).First(m => m.Name == statement.GetTextAt(7));
        var move4 = (await FetchMoveSearchDataAsync()).First(m => m.Name == statement.GetTextAt(8));
        var ability = (await FetchAbilitySearchDataAsync()).First(a => a.Name == statement.GetTextAt(9));
        var item = (await FetchItemSearchDataAsync()).First(i => i.Name == statement.GetTextAt(10));

        var nature = (Nature) statement.GetIntAt(11);
        var level = statement.GetIntAt(12);

        var ev = new BaseStat
        {
          HP = statement.GetIntAt(13),
          Attack = statement.GetIntAt(14),
          Defense = statement.GetIntAt(15),
          SpecialAttack = statement.GetIntAt(16),
          SpecialDefense = statement.GetIntAt(17),
          Speed = statement.GetIntAt(18),
        };

        var iv = new BaseStat
        {
          HP = statement.GetIntAt(19),
          Attack = statement.GetIntAt(20),
          Defense = statement.GetIntAt(21),
          SpecialAttack = statement.GetIntAt(22),
          SpecialDefense = statement.GetIntAt(23),
          Speed = statement.GetIntAt(24),
        };

        currentTeam.TeamMembers.Add(new TeamMember
        {
          ID = memberId,
          Pokemon = pokemon,
          Moves = new List<Move>{move1, move2, move3, move4},
          Ability = ability,
          Item = item,
          Nature = nature,
          Level = level,
          EVSpread = ev
        });
      }

      if (currentTeam != null)
      {
        // add the last one
        teams.Add(currentTeam);
      }

      return teams;
    }

    public async Task<Team> CreateNewTeamAsync(string teamName, TeamType teamType)
    {
      ensureDatabaseInitialized();

      var team = new Team
      {
        Name = teamName,
        TeamType = teamType,
        TeamMembers = new List<TeamMember>()
      };

      var statement = await _database.PrepareStatementAsync(InsertTeamQuery);
      statement.BindTextParameterWithName("@name", teamName);
      statement.BindIntParameterWithName("@type", (int)teamType);

      statement.StepSync();

      team.ID = (int) _database.GetLastInsertedRowId();

      return team;
    }

    public async Task UpdateTeamAsync(Team team)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(UpdateTeamQuery);
      statement.BindTextParameterWithName("@name", team.Name);
      statement.BindIntParameterWithName("@type", (int)team.TeamType);
      statement.BindIntParameterWithName("@id", team.ID);

      statement.StepSync();
    }

    public async Task DeleteTeamAsync(Team team)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(DeleteAllTeamMembersFromTeamQuery);
      statement.BindIntParameterWithName("@teamid", team.ID);
      statement.StepSync();

      statement = await _database.PrepareStatementAsync(DeleteTeamQuery);
      statement.BindIntParameterWithName("@id", team.ID);
      statement.StepSync();
    }

    public async Task AddMemberToTeamAsync(Team team, TeamMember member)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(InsertTeamMemberQuery);
      statement.BindIntParameterWithName("@teamid", team.ID);
      statement.BindTextParameterWithName("@pokemon", member.Pokemon.Name);
      statement.BindTextParameterWithName("@move1", member.Moves[0].Name);
      statement.BindTextParameterWithName("@move2", member.Moves[1].Name);
      statement.BindTextParameterWithName("@move3", member.Moves[2].Name);
      statement.BindTextParameterWithName("@move4", member.Moves[3].Name);
      statement.BindTextParameterWithName("@ability", member.Ability.Name);
      statement.BindTextParameterWithName("@item", member.Item.Name);
      statement.BindIntParameterWithName("@nature", (int)member.Nature);
      statement.BindIntParameterWithName("@level", member.Level);
      statement.BindIntParameterWithName("@ehp", member.EVSpread.HP);
      statement.BindIntParameterWithName("@eatk", member.EVSpread.Attack);
      statement.BindIntParameterWithName("@edef", member.EVSpread.Defense);
      statement.BindIntParameterWithName("@espa", member.EVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@espd", member.EVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@espe", member.EVSpread.Speed);
      statement.BindIntParameterWithName("@ihp", member.IVSpread.HP);
      statement.BindIntParameterWithName("@iatk", member.IVSpread.Attack);
      statement.BindIntParameterWithName("@idef", member.IVSpread.Defense);
      statement.BindIntParameterWithName("@ispa", member.IVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@ispd", member.IVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@ispe", member.IVSpread.Speed);
      statement.StepSync();
    }

    public async Task UpdateTeamMember(TeamMember member)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(UpdateTeamMemberQuery);
      statement.BindIntParameterWithName("@id", member.ID);
      statement.BindTextParameterWithName("@pokemon", member.Pokemon.Name);
      statement.BindTextParameterWithName("@move1", member.Moves[0].Name);
      statement.BindTextParameterWithName("@move2", member.Moves[1].Name);
      statement.BindTextParameterWithName("@move3", member.Moves[2].Name);
      statement.BindTextParameterWithName("@move4", member.Moves[3].Name);
      statement.BindTextParameterWithName("@ability", member.Ability.Name);
      statement.BindTextParameterWithName("@item", member.Item.Name);
      statement.BindIntParameterWithName("@nature", (int)member.Nature);
      statement.BindIntParameterWithName("@level", member.Level);
      statement.BindIntParameterWithName("@ehp", member.EVSpread.HP);
      statement.BindIntParameterWithName("@eatk", member.EVSpread.Attack);
      statement.BindIntParameterWithName("@edef", member.EVSpread.Defense);
      statement.BindIntParameterWithName("@espa", member.EVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@espd", member.EVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@espe", member.EVSpread.Speed);
      statement.BindIntParameterWithName("@ihp", member.IVSpread.HP);
      statement.BindIntParameterWithName("@iatk", member.IVSpread.Attack);
      statement.BindIntParameterWithName("@idef", member.IVSpread.Defense);
      statement.BindIntParameterWithName("@ispa", member.IVSpread.SpecialAttack);
      statement.BindIntParameterWithName("@ispd", member.IVSpread.SpecialDefense);
      statement.BindIntParameterWithName("@ispe", member.IVSpread.Speed);
      statement.StepSync();
    }

    public async Task MoveTeamMember(TeamMember member, Team to)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(UpdateTeamMemberTeamQuery);
      statement.BindIntParameterWithName("@teamid", to.ID);
      statement.BindIntParameterWithName("@id", member.ID);
    }

    public async Task DeleteTeamMember(TeamMember member)
    {
      ensureDatabaseInitialized();

      var statement = await _database.PrepareStatementAsync(DeleteTeamMemberQuery);
      statement.BindIntParameterWithName("@id", member.ID);
      statement.StepSync();
    }
  }

}
