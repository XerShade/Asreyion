using Asreyion.Core.Files.Services;
using Asreyion.Core.Files.Services.Interfaces;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;

namespace Asreyion.Core.Files.Hooks;

/// <summary>
/// Adds shared file upload and download services.
/// </summary>
[HookName("File Upload Service")]
[HookDescription("Adds shared file upload and download services.")]
public sealed class FileServiceHooks : IConfigureServicesHook
{
    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration) 
        => services.AddScoped<IFileUploadService, FileUploadService>();
}
