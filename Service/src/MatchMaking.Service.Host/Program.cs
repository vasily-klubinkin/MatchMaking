using System.Net;
using MatchMaking.Service.Application;
using MatchMaking.Service.Host.Constants;
using MatchMaking.Service.Infra;
using MatchMaking.Service.Infra.Options;
using Microsoft.OpenApi.Models;
using RedisRateLimiting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Matchmaking API",
        Version = "v1"
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.AddPolicy("user-id-policy",
                      policy =>
                          RedisRateLimitPartition
                              .GetSlidingWindowRateLimiter(partitionKey: policy.Request.Headers[HeaderNames.UserIdHeaderName].ToString(),
                                                           factory: _ => new RedisSlidingWindowRateLimiterOptions
                                                           {
                                                               PermitLimit = 1,
                                                               Window = TimeSpan.FromMilliseconds(100),
                                                               ConnectionMultiplexerFactory = () =>
                                                                   ConnectionMultiplexer.Connect(builder
                                                                       .Configuration
                                                                       .GetSection(RedisOptions.SectionName)
                                                                       .Get<RedisOptions>()!.ConnectionString),
                                                           }));
});

builder.Services.AddControllers();

builder.Services
    .AddApplication()
    .AddInfra(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Matchmaking API v1");
    c.RoutePrefix = "swagger";
});

app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("user-id-policy");

app.Run();