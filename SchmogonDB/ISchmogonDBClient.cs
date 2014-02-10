using System.Collections.Generic;
using System.Threading.Tasks;
using SchmogonDB.Model.Abilities;
using SchmogonDB.Model.Items;
using SchmogonDB.Model.Moves;
using SchmogonDB.Model.Pokemon;
using SchmogonDB.Model.Teams;
using SchmogonDB.Model.Text;
using SchmogonDB.Population;

namespace SchmogonDB
{
  public interface ISchmogonDBClient
  {
    Task<IEnumerable<Item>> FetchItemSearchDataAsync();

    Task<ItemData> FetchItemDataAsync(Item item);

    Task<IEnumerable<Move>> FetchMoveSearchDataAsync();

    Task<MoveData> FetchMoveDataAsync(Move move);

    Task<IEnumerable<Pokemon>> FetchPokemonSearchDataAsync();

    Task<PokemonData> FetchPokemonDataAsync(Pokemon pokemon);

    Task<IEnumerable<Ability>> FetchAbilitySearchDataAsync();

    Task<AbilityData> FetchAbilityDataAsync(Ability ability);

    Task<IEnumerable<ITextElement>> FetchTextElementsAsync(string ownerId, OwnerType ownerType, ElementType elementType);

    Task<IEnumerable<Team>> FetchAllTeamsAsync();
    Task<Team> CreateNewTeamAsync(string teamName, TeamType teamType);
    Task UpdateTeamAsync(Team team);
    Task DeleteTeamAsync(Team team);
    Task AddMemberToTeamAsync(Team team, TeamMember member);
    Task UpdateTeamMember(TeamMember member);
    Task MoveTeamMember(TeamMember member, Team to);
    Task DeleteTeamMember(TeamMember member);

    Task InitializeDatabase();
  }

  public interface ITeamBuilderClient
  {

  }
}