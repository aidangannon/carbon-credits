using Application.Ports;
using Core.Errors;
using Core.Models;
using Crosscutting.Result;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Persistence.Adapters;

public class FileProjectRepository(IOptions<FileOptions> fileOptions, IFileStore fileStore) : IProjectRepository
{
    public async Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/projects/{id}";

        var result = await fileStore.GetAsync<Project>(path, cancellationToken);

        return result.HasFailed()
            ? Result<Project>.Err(ProjectErrors.NotFound)
            : Result<Project>.Ok(result.Unwrap());
    }

    public async Task<Result> SaveAsync(Project project, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/projects/{project.Id}";

        return await fileStore.SaveAsync(path, project, cancellationToken);
    }
}
