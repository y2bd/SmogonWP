﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Natures;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Stats;
using SchmogonDB.Model.Teams;
using Type = SchmogonDB.Model.Types.Type;

namespace SmogonWP.Design
{
  public class DesignTeamBuilderClient : ITeamBuilderClient
  {
    public async Task<IEnumerable<Team>> FetchAllTeamsAsync()
    {
      return new List<Team>
      {
        await CreateNewTeamAsync("hello", TeamType.Singles),
        await CreateNewTeamAsync("goodday", TeamType.Doubles),
        await CreateNewTeamAsync("woopwoop", TeamType.Triples),
        await CreateNewTeamAsync("muchies", TeamType.Rotation),
      };
    }

    public async Task<Team> CreateNewTeamAsync(string teamName, TeamType teamType)
    {
      await Task.Delay(0);

      return new Team("John", TeamType.Singles, new List<TeamMember>
      {
        new TeamMember
        {
          Ability = new Ability("kung pao", "", ""),
          EVSpread = new BaseStat(0, 0, 0, 252, 6, 252),
          Item = new Item("meat", "", ""),
          Level = 100,
          Moves = new List<Move>
          {
            new Move("fiddlesticks", "", "", Type.Fire),
            new Move("tenderizer", "", "", Type.Fighting),
            new Move("blowdoom", "", "", Type.Dark),
            new Move("shootgun", "", "", Type.Steel),
          },
          Nature = Nature.Timid,
          Pokemon =
            new Pokemon("jackson", new List<Type> {Type.Fire}, Tier.OU,
              new List<Ability> {new Ability("kung pao", "", "")}, new BaseStat(0, 0, 0, 252, 6, 252), ""),
        },
        new TeamMember
        {
          Ability = new Ability("kung pao", "", ""),
          EVSpread = new BaseStat(0, 0, 0, 252, 6, 252),
          Item = new Item("meat", "", ""),
          Level = 100,
          Moves = new List<Move>
          {
            new Move("fiddlesticks", "", "", Type.Fire),
            new Move("tenderizer", "", "", Type.Fighting),
            new Move("blowdoom", "", "", Type.Dark),
            new Move("shootgun", "", "", Type.Steel),
          },
          Nature = Nature.Timid,
          Pokemon =
            new Pokemon("jackson", new List<Type> {Type.Fire}, Tier.OU,
              new List<Ability> {new Ability("kung pao", "", "")}, new BaseStat(0, 0, 0, 252, 6, 252), ""),
        },
        new TeamMember
        {
          Ability = new Ability("kung pao", "", ""),
          EVSpread = new BaseStat(0, 0, 0, 252, 6, 252),
          Item = new Item("meat", "", ""),
          Level = 100,
          Moves = new List<Move>
          {
            new Move("fiddlesticks", "", "", Type.Fire),
            new Move("tenderizer", "", "", Type.Fighting),
            new Move("blowdoom", "", "", Type.Dark),
            new Move("shootgun", "", "", Type.Steel),
          },
          Nature = Nature.Timid,
          Pokemon =
            new Pokemon("jackson", new List<Type> {Type.Fire}, Tier.OU,
              new List<Ability> {new Ability("kung pao", "", "")}, new BaseStat(0, 0, 0, 252, 6, 252), ""),
        },
        new TeamMember
        {
          Ability = new Ability("kung pao", "", ""),
          EVSpread = new BaseStat(0, 0, 0, 252, 6, 252),
          Item = new Item("meat", "", ""),
          Level = 100,
          Moves = new List<Move>
          {
            new Move("fiddlesticks", "", "", Type.Fire),
            new Move("tenderizer", "", "", Type.Fighting),
            new Move("blowdoom", "", "", Type.Dark),
            new Move("shootgun", "", "", Type.Steel),
          },
          Nature = Nature.Timid,
          Pokemon =
            new Pokemon("jackson", new List<Type> {Type.Fire}, Tier.OU,
              new List<Ability> {new Ability("kung pao", "", "")}, new BaseStat(0, 0, 0, 252, 6, 252), ""),
        }
      });
    }

    public async Task UpdateTeamAsync(Team team)
    {
      await Task.Delay(0);
    }

    public async Task DeleteTeamAsync(Team team)
    {
      await Task.Delay(0);
    }

    public async Task AddMemberToTeamAsync(Team team, TeamMember member)
    {
      await Task.Delay(0);
    }

    public async Task UpdateTeamMember(TeamMember member)
    {
      await Task.Delay(0);
    }

    public async Task MoveTeamMember(TeamMember member, Team to)
    {
      await Task.Delay(0);
    }

    public async Task DeleteTeamMember(TeamMember member)
    {
      await Task.Delay(0);
    }
  }
}