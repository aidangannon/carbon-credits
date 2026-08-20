using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Projects;

public interface IProjectCreationService
{
    Task<Result<Project>> CreateProject(Project project, CancellationToken cancellationToken);
}

public class ProjectCreationService(IProjectRepository projectRepository) : IProjectCreationService
{
    public async Task<Result<Project>> CreateProject(Project project, CancellationToken cancellationToken)
    {
        var result = await projectRepository.SaveAsync(project, cancellationToken);

        return result.HasFailed() ? Result<Project>.Err(result.Error) : Result<Project>.Ok(project);
    }
}
