using Confluent.Kafka;
using MatchMaking.Worker.Application.POCO;
using MatchMaking.Worker.Application.Services.Base;
using MatchMaking.Worker.Infra.Kafka.Base;
using MatchMaking.Worker.Infra.Options;
using MatchMaking.Worker.Infra.POCO;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Worker.Infra.Consumers;

internal class MatchmakingRequestKafkaConsumer : KafkaConsumerBase<MatchmakingRequest, DummyKey, string>
{
    private readonly IMatchmakingService _service;
    
    public MatchmakingRequestKafkaConsumer(
        KafkaOptions kafkaOptions,
        IKafkaFactory kafkaFactory,
        ILogger<MatchmakingRequestKafkaConsumer> logger,
        IMatchmakingService service): base(kafkaFactory, kafkaOptions.MatchmakingRequestTopic, logger)
    {
        _service = service;
    }

    protected override MatchmakingRequest CreateMessage(DummyKey _, string value, ConsumeResult<string, string> originalMessage)
    {
        return new MatchmakingRequest(value, originalMessage.Partition.Value);
    }

    protected override async Task<bool> HandleMessageAsync(MatchmakingRequest message, CancellationToken cancellationToken)
    {
        var formedMatch = await _service.HandleMatchmakingRequestAsync(message, cancellationToken);
        
        return formedMatch is not null;
    }
}