using Asreyion.Core.Configuration;
using Asreyion.Core.Hooks.Extensions;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(HostConfiguration.Create(args));

builder.Services.DiscoverHooks(Assembly.GetExecutingAssembly());
builder.Services.ConfigureServicesWithHooks(builder.Configuration);

WebApplication app = builder.Build();

app.ConfigureApplicationWithHooks();

app.Run();
