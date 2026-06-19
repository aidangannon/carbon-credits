using System.Text.Json;
using Application.Ports;
using Core.Errors;
using Core.Models;
using Crosscutting.Result;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Persistence.Adapters;

public class FileProjectRepository(IOptions<FileOptions> fileOptions) : IProjectRepository
{
    public async Task<Result> CreateAsync(Project project, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/projects/{project.Id}";

        var projectText = JsonSerializer.Serialize(project);
        await File.WriteAllTextAsync(path, projectText, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/projects/{project.Id}";

        if (!File.Exists(path))
        {
            return Result.Err(ProjectErrors.NotFound);
        }

        var projectText = JsonSerializer.Serialize(project);
        await File.WriteAllTextAsync(path, projectText, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/projects/{id}";

        if (!File.Exists(path))
        {
            return Result<Project>.Err(ProjectErrors.NotFound);
        }

        var projectText = await File.ReadAllTextAsync(path, cancellationToken);
        var project = JsonSerializer.Deserialize<Project>(projectText);

        return Result<Project>.Ok(project!);
    }
}
