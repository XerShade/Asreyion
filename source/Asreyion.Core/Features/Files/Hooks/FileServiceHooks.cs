using Asreyion.Core.Features.Files.Services;
using Asreyion.Core.Features.Files.Services.Interfaces;
using Asreyion.Core.Features.Hooks.Hooks;

namespace Asreyion.Core.Features.Files.Hooks;

public sealed class FileServiceHooks : IConfigureServicesHook
{
    public string Name => "File Upload Service";

    public string Description => "Adds shared file upload and download services.";

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileUploadService, FileUploadService>();
    }
}
