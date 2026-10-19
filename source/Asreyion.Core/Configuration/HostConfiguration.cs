using System.Reflection;

namespace Asreyion.Core.Configuration;

/// <summary>
/// Creates the host options used by the Asreyion web application.
/// </summary>
public static class HostConfiguration
{
    /// <summary>
    /// Builds web application options using the server assembly location and host environment.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>Options configured with content and web root paths for the server.</returns>
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
