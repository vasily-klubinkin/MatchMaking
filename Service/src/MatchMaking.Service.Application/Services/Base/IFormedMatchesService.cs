using MatchMaking.Service.Application.POCO;

namespace MatchMaking.Service.Application.Services.Base;

public interface IFormedMatchesService
{
    Task AddAsync(FormedMatch formedMatch);
    Task<FormedMatch?> Get(string userId);
}