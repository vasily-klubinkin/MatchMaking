using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Application.Publisher;
using MatchMaking.Service.Application.Repositories;
using MatchMaking.Service.Infra.Consumers;
using MatchMaking.Service.Infra.Kafka;
using MatchMaking.Service.Infra.Kafka.Base;
using MatchMaking.Service.Infra.Options;
using MatchMaking.Service.Infra.Publishers;
using MatchMaking.Service.Infra.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MatchMaking.Service.Infra;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
        services.AddSingleton(redisOptions);
        
        var kafkaOptions = configuration.GetSection(KafkaOptions.SectionName).Get<KafkaOptions>();
        services.AddSingleton(kafkaOptions);
        
        services.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect(redisOptions.ConnectionString));
        
        services.AddSingleton<IUsersQueueRepository, UsersQueueRedisRepository>();
        services.AddSingleton<IFormedMatchesRepository, FormedMatchesRedisRepository>();

        services.AddSingleton<IKafkaFactory, KafkaFactory>();

        services.AddSingleton<IMessagePublisher<MatchmakingRequest>, MatchmakingRequestKafkaPublisher>();
        
        services.AddSingleton<FormedMatchesKafkaConsumer>();
        services.AddHostedService<FormedMatchesKafkaConsumer>();
        
        return services;
    }
}