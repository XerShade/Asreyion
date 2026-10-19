using Asreyion.Core.Features.Files.Services;
using Asreyion.Core.Features.Files.Services.Interfaces;
using Asreyion.Core.Features.Hooks.Hooks;

namespace Asreyion.Core.Features.Files.Hooks;

/// <summary>
/// Registers the shared file upload service.
/// </summary>
public sealed class FileServiceHooks : IConfigureServicesHook
{
    /// <inheritdoc />
    public string Name => "File Upload Service";

    /// <inheritdoc />
    public string Description => "Adds shared file upload and download services.";

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileUploadService, FileUploadService>();
    }
}
