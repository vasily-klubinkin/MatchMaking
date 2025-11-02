using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Application.Repositories;
using MatchMaking.Service.Application.Services.Base;

namespace MatchMaking.Service.Application.Services;

public class FormedMatchesService : IFormedMatchesService
{
    private readonly IFormedMatchesRepository _formedMatchesRepository;
    private readonly IUsersQueueRepository _usersQueueRepository;

    public FormedMatchesService(IFormedMatchesRepository formedMatchesRepository, IUsersQueueRepository usersQueueRepository)
    {
        _formedMatchesRepository = formedMatchesRepository;
        _usersQueueRepository = usersQueueRepository;
    }

    public async Task AddAsync(FormedMatch formedMatch)
    {
        await _formedMatchesRepository.AddMatch(formedMatch);
        
        await _usersQueueRepository.ClearUsersInQueue(formedMatch.UserIds);
    }

    public Task<FormedMatch?> Get(string userId)
    {
        return _formedMatchesRepository.GetUserMatch(userId);
    }
}