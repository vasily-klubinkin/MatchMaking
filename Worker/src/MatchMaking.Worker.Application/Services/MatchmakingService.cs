using MatchMaking.Worker.Application.Options;
using MatchMaking.Worker.Application.POCO;
using MatchMaking.Worker.Application.Publisher;
using MatchMaking.Worker.Application.Repositories;
using MatchMaking.Worker.Application.Services.Base;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Worker.Application.Services;

// we could build this using Rx, 
// but that would make testing and maintenance more complex 
// alternatively, we can use the TPL/DataFlow for async processing and task coordination.
public class MatchmakingService : IMatchmakingService
{
    private readonly MatchOptions _matchOptions;
    private readonly IMessagePublisher<FormedMatch> _formedMatchesPublisher;
    private readonly IUserQueuesRepository _userQueuesRepository;
    private readonly ILogger<MatchmakingService> _logger;

    public MatchmakingService(
        IUserQueuesRepository userQueuesRepository,
        MatchOptions matchOptions,
        IMessagePublisher<FormedMatch> formedMatchesPublisher,
        ILogger<MatchmakingService> logger)
    {
        _userQueuesRepository = userQueuesRepository;
        _matchOptions = matchOptions;
        _formedMatchesPublisher = formedMatchesPublisher;
        _logger = logger;
    }
    
    // next improvement point will be to parallelize it by UserProps
    public async Task<FormedMatch?> HandleMatchmakingRequestAsync(MatchmakingRequest request, CancellationToken cancellationToken)
    {
        var currentWaitingUsersCount = await _userQueuesRepository.GetUsersCountAsync(request.QueueId);
        
        // queue is too small, need to w8 more users to join
        if (currentWaitingUsersCount + 1 < _matchOptions.RequiredUsersCount)
        {
            await _userQueuesRepository.EnqueueUserAsync(request.QueueId, request.UserId);
            return null;
        }
        
        var waitingUsers = await _userQueuesRepository.DequeueUsersAsync(request.QueueId, _matchOptions.RequiredUsersCount - 1);
        waitingUsers.Add(request.UserId);
        
        var formedMatch = new FormedMatch(Guid.NewGuid().ToString("N"), waitingUsers);
        
        await _formedMatchesPublisher.PublishAsync(formedMatch, cancellationToken);
        
        _logger.LogInformation("Match formed succesfully. MatchId: {matchId}", formedMatch.MatchId);
        
        return formedMatch;
    }
}