using MatchMaking.Worker.Application;
using MatchMaking.Worker.Infra;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfra(builder.Configuration);

var host = builder.Build();
host.Run();