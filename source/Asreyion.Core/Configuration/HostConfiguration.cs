using System.Reflection;

namespace Asreyion.Core.Configuration;

public static class HostConfiguration
{
    public static WebApplicationOptions Create(string[] args)
    {
        string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        return new WebApplicationOptions
        {
            ContentRootPath = assemblyPath,

            WebRootPath = Path.Combine(assemblyPath, "wwwroot"),

            Args = args,

            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",

            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name ?? "Asreyion"
        };
    }
}