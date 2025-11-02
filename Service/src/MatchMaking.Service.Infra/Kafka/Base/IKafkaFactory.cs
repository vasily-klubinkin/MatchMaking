using Confluent.Kafka;

namespace MatchMaking.Service.Infra.Kafka.Base;

public interface IKafkaFactory
{
    IProducer<string, string> CreateProducer();
    IConsumer<string, string> CreateConsumer();
}