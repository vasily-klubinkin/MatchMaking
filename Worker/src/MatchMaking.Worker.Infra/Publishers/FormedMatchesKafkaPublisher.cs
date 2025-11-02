using MatchMaking.Worker.Application.POCO;
using MatchMaking.Worker.Infra.Kafka;
using MatchMaking.Worker.Infra.Kafka.Base;
using MatchMaking.Worker.Infra.Options;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Worker.Infra.Publishers;

internal class FormedMatchesKafkaPublisher : KafkaPublisherBase<FormedMatch, string, FormedMatch>
{
    public FormedMatchesKafkaPublisher(
        KafkaOptions options,
        IKafkaFactory kafkaFactory,
        ILogger<FormedMatchesKafkaPublisher> logger): base(options.MatchmakingCompleteTopic, kafkaFactory, logger)
    {
    }

    protected override string GetKey(FormedMatch value)
    {
        return value.MatchId;
    }

    protected override FormedMatch GetValue(FormedMatch value)
    {
        return value;
    }
}