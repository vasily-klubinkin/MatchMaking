using MatchMaking.Service.Application.Services;
using MatchMaking.Service.Application.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Service.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMatchmakingService, MatchmakingService>();
        services.AddSingleton<IFormedMatchesService, FormedMatchesService>();
        
        return services;
    }
}