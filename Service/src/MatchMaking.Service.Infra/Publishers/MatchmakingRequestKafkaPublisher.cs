using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Infra.Kafka.Base;
using MatchMaking.Service.Infra.Options;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Service.Infra.Publishers;

internal class MatchmakingRequestKafkaPublisher : KafkaPublisherBase<MatchmakingRequest, UserProps, string>
{
    public MatchmakingRequestKafkaPublisher(
        KafkaOptions kafkaOptions,
        IKafkaFactory kafkaFactory,
        ILogger<MatchmakingRequestKafkaPublisher> logger): base(kafkaOptions.MatchmakingRequestsTopic, kafkaFactory, logger)
    {
    }

    protected override UserProps GetKey(MatchmakingRequest value) => value.UserProperties;

    protected override string GetValue(MatchmakingRequest value) => value.UserId;
}