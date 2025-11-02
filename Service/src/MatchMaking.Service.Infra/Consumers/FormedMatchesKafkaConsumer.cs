using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Application.Services.Base;
using MatchMaking.Service.Infra.Kafka.Base;
using MatchMaking.Service.Infra.Options;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Service.Infra.Consumers;

public class FormedMatchesKafkaConsumer : KafkaConsumerBase<FormedMatch, string, FormedMatch>
{
    private readonly IFormedMatchesService _formedMatchesService;
    
    public FormedMatchesKafkaConsumer(
        IKafkaFactory kafkaFactory,
        KafkaOptions kafkaOptions,
        ILogger<FormedMatchesKafkaConsumer> logger,
        IFormedMatchesService formedMatchesService): base(kafkaFactory, kafkaOptions.CompletedMatchesTopic, logger)
    {
        _formedMatchesService = formedMatchesService;
    }

    protected override FormedMatch CreateMessage(string key, FormedMatch value)
    {
        return value;
    }

    protected override async Task<bool> HandleMessageAsync(FormedMatch message)
    {
        await _formedMatchesService.AddAsync(message);
        
        return true;
    }
}