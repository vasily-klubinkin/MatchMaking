using MatchMaking.Service.Application.POCO;

namespace MatchMaking.Service.Application.Repositories;

public interface IFormedMatchesRepository
{
    Task AddMatch(FormedMatch formedMatch, CancellationToken cancellationToken);
    
    Task<FormedMatch?> GetUserMatch(string userId);
}