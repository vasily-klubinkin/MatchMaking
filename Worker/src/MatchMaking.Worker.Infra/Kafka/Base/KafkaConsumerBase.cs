using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Worker.Infra.Kafka.Base;

internal abstract class KafkaConsumerBase<TDomain, TKey, TValue> : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    protected KafkaConsumerBase(
        IKafkaFactory kafkaFactory,
        string topic,
        ILogger logger)
    {
        _consumer = kafkaFactory.CreateConsumer();
        _topic = topic;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    protected abstract TDomain CreateMessage(TKey key, TValue value, ConsumeResult<string, string> originalMessage);
    protected abstract Task<bool> HandleMessageAsync(TDomain message);
    
    

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Factory.StartNew(
                              () => RunConsumerLoopAsync(stoppingToken),
                              stoppingToken,
                              TaskCreationOptions.LongRunning,
                              TaskScheduler.Default);
        return Task.CompletedTask;
    }
    
    private async Task RunConsumerLoopAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Kafka consumer subscribed to topic {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result?.Message?.Value == null)
                        continue;

                    TDomain? message;
                    try
                    {
                        var key = JsonSerializer.Deserialize<TKey>(result.Message.Key, _jsonSerializerOptions);
                        var value = JsonSerializer.Deserialize<TValue>(result.Message.Value, _jsonSerializerOptions);
                        if (key is null || value is null)
                        {
                            _logger.LogError("Either key {key} or value {value} of kafka message is null from topic {Topic}.", key, value, _topic);
                            continue;
                        }
                        
                        message = CreateMessage(key, value, originalMessage: result);
                    }
                    catch (Exception jsonEx)
                    {
                        // if needed we can make it configurable on "concrete" consumer level, but for current task
                        // it looks more important to make a "progress" and do not freeze in case of single broken message
                        // cause it affect only one person, but everyone else in the partition will make a progress
                        _logger.LogError(jsonEx, "Failed to deserialize message from topic {Topic}. Message {Message}", _topic, result.Message.Value);
                        continue;
                    }

                    var commit = await HandleMessageAsync(message!);
                    if (commit) _consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error on topic {Topic}", _topic);
                }
                catch (OperationCanceledException)
                {
                    // graceful shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while processing message from topic {Topic}", _topic);
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Kafka consumer for topic {Topic} closed", _topic);
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}