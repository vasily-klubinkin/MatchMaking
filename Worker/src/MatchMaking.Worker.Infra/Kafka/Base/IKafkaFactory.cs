using Confluent.Kafka;

namespace MatchMaking.Worker.Infra.Kafka.Base;

internal interface IKafkaFactory
{
    IProducer<string, string> CreateProducer();
    IConsumer<string, string> CreateConsumer();
}