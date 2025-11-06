using Confluent.Kafka;
using MatchMaking.Service.Infra.Kafka.Base;
using MatchMaking.Service.Infra.Options;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Service.Infra.Kafka;

internal class KafkaFactory : IKafkaFactory
{
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaFactory> _logger;

    public KafkaFactory(KafkaOptions options, ILogger<KafkaFactory> logger)
    {
        _options = options;
        _logger = logger;
    }

    public IConsumer<string, string> CreateConsumer()
    {
        var baseConfig = new ConsumerConfig();

        if (_options.ConsumerConfig is { Count: > 0 })
        {
            foreach (var kv in _options.ConsumerConfig) baseConfig.Set(kv.Key, kv.Value);
        }
        
        // higher priority than ConsumerConfig
        baseConfig.BootstrapServers = _options.BootstrapServers;
        baseConfig.GroupId = _options.ConsumerGroup;

        var consumer = new ConsumerBuilder<string, string>(baseConfig)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka consumer error: {Error}", e.Reason))
            .Build();

        return consumer;
    }

    public IProducer<string, string> CreateProducer()
    {
        var baseConfig = new ProducerConfig();

        if (_options.ProducerConfig is { Count: > 0 })
        {
            foreach (var kv in _options.ProducerConfig) baseConfig.Set(kv.Key, kv.Value);
        }

        // higher priority than ProducerConfig
        baseConfig.BootstrapServers = _options.BootstrapServers;
        
        var producer = new ProducerBuilder<string, string>(baseConfig)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka producer error: {Error}", e.Reason))
            .Build();

        return producer;
    }
}
