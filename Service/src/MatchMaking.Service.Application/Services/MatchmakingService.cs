using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Application.Publisher;
using MatchMaking.Service.Application.Repositories;
using MatchMaking.Service.Application.Services.Base;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Service.Application.Services;

public class MatchmakingService : IMatchmakingService
{
    private readonly IUsersQueueRepository _usersQueueRepository;
    private readonly IMessagePublisher<MatchmakingRequest> _matchmakingRequestsPublisher;
    private readonly ILogger<MatchmakingService> _logger;

    public MatchmakingService(
        IUsersQueueRepository usersQueueRepository,
        IMessagePublisher<MatchmakingRequest> matchmakingRequestsPublisher,
        ILogger<MatchmakingService> logger)
    {
        _usersQueueRepository = usersQueueRepository;
        _matchmakingRequestsPublisher = matchmakingRequestsPublisher;
        _logger = logger;
    }

    public async Task SearchMatchForUser(string userId, CancellationToken cancellationToken)
    {
        var isUserInQueue = await _usersQueueRepository.IsUserInQueue(userId);
        if (isUserInQueue) return;

        await
            _matchmakingRequestsPublisher.PublishAsync(new MatchmakingRequest(userId, GenerateRandomUserProps(userId)),
                                                       cancellationToken);
        
        // possible duplicates in kafka, but it is expected
        await _usersQueueRepository.AddUserToQueue(userId);
        
        _logger.LogInformation("User {userId} has been added to queue", userId);
    }

    // actually for this approach we can use even userId for partitioning, but using some
    // kind of region or rank looks more logical
    private static UserProps GenerateRandomUserProps(string userId)
    {
        string[] availableRegions = { "eu-central", "eu-west", "us-east", "us-west", "asia" };

        // to guarantee that the same userId will be assigned the same region
        int index = Math.Abs(userId.GetHashCode()) % availableRegions.Length;
        var region = availableRegions[index];

        return new UserProps(region);
    }
}