using System.Text.Json;
using Confluent.Kafka;
using MatchMaking.Service.Application.Publisher;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Service.Infra.Kafka.Base;

public abstract class KafkaPublisherBase<TDomain, TKey, TValue> : IMessagePublisher<TDomain>
{
    private readonly string _topic;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public KafkaPublisherBase(
        string topic,
        IKafkaFactory kafkaFactory,
        ILogger logger)
    {
        _topic = topic;
        _producer = kafkaFactory.CreateProducer();
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }
    
    protected abstract TKey GetKey(TDomain value);
    protected abstract TValue GetValue(TDomain value);
    
    public async Task PublishAsync(TDomain message)
    {
        try
        {
            var key = JsonSerializer.Serialize(GetKey(message), _jsonSerializerOptions);
            var value = JsonSerializer.Serialize(GetValue(message), _jsonSerializerOptions);

            var kafkaMessage = new Message<string, string> { Key = key, Value = value };

            var deliveryResult = await _producer.ProduceAsync(_topic, kafkaMessage);

            _logger.LogInformation("Message published to {Topic} [Partition={Partition}, Offset={Offset}]",
                                   deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to topic {Topic}", _topic);
            throw;
        }
    }
}