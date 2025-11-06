using MatchMaking.Worker.Application.Options;
using MatchMaking.Worker.Application.Services;
using MatchMaking.Worker.Application.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Worker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var matchOptions = configuration.GetSection(MatchOptions.SectionName).Get<MatchOptions>();
        services.AddSingleton(matchOptions!);
        
        services.AddSingleton<IMatchmakingService, MatchmakingService>();
        services.Decorate<IMatchmakingService, ParallelMatchmakingServiceDecorator>();
        
        return services;
    }
}