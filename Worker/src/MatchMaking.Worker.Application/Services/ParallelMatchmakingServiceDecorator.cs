using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using MatchMaking.Worker.Application.POCO;
using MatchMaking.Worker.Application.Services.Base;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Worker.Application.Services;

internal class ParallelMatchmakingServiceDecorator : IMatchmakingService, IDisposable
{
    private readonly IMatchmakingService _inner;
    private readonly ILogger<ParallelMatchmakingServiceDecorator> _logger;
    
    private readonly ConcurrentDictionary<int, ActionBlock<MatchmakingRequestWithTcs>> _blocks = new();

    public ParallelMatchmakingServiceDecorator(
        IMatchmakingService inner,
        ILogger<ParallelMatchmakingServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public Task<FormedMatch?> HandleMatchmakingRequestAsync(MatchmakingRequest request)
    {
        var block = _blocks.GetOrAdd(request.QueueId, up =>
        {
            return new ActionBlock<MatchmakingRequestWithTcs>(
                async rwt => await ProcessRequestAsync(rwt),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,
                    EnsureOrdered = true
                });
        });

        var tcs = new TaskCompletionSource<FormedMatch?>();
        block.Post(new MatchmakingRequestWithTcs(request, tcs));
        return tcs.Task;
    }

    private async Task ProcessRequestAsync(MatchmakingRequestWithTcs rwt)
    {
        try
        {
            var result = await _inner.HandleMatchmakingRequestAsync(rwt.Request);
            rwt.Tcs.SetResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing matchmaking request");
            rwt.Tcs.SetException(ex);
        }
    }

    public void Dispose()
    {
        foreach (var block in _blocks.Values)
        {
            block.Complete();
        }
    }

    private record MatchmakingRequestWithTcs(MatchmakingRequest Request, TaskCompletionSource<FormedMatch?> Tcs);
}
