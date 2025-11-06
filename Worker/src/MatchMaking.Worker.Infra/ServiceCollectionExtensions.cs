using MatchMaking.Worker.Application.POCO;
using MatchMaking.Worker.Application.Publisher;
using MatchMaking.Worker.Application.Repositories;
using MatchMaking.Worker.Infra.Consumers;
using MatchMaking.Worker.Infra.Kafka;
using MatchMaking.Worker.Infra.Kafka.Base;
using MatchMaking.Worker.Infra.Options;
using MatchMaking.Worker.Infra.Publishers;
using MatchMaking.Worker.Infra.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Worker.Infra;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaOptions = configuration.GetSection(KafkaOptions.SectionName).Get<KafkaOptions>();
        services.AddSingleton(kafkaOptions!);

        services.AddSingleton<IUserQueuesRepository, UserQueuesInMemoryRepository>();

        services.AddSingleton<IKafkaFactory, KafkaFactory>();

        services.AddSingleton<MatchmakingRequestKafkaConsumer>();
        services.AddHostedService<MatchmakingRequestKafkaConsumer>();

        services.AddSingleton<IMessagePublisher<FormedMatch>, FormedMatchesKafkaPublisher>();
        
        return services;
    }
}