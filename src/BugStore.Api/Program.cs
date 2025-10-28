using BugStore.Api.Extensions;
using BugStore.Application.Extensions;
using BugStore.Endpoints;
using BugStore.Infrastructure.Extensions;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJsonConfiguration();
var app = builder.Build();

app.MapEndpoints();

await app.Services.ApplyMigrationsAsync();

app.Run();
