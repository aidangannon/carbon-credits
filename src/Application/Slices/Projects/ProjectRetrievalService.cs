using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Projects;

public interface IProjectRetrievalService
{
    Task<Result<Project>> GetProjectById(Guid id, CancellationToken cancellationToken);
}

public class ProjectRetrievalService(IProjectRepository projectRepository) : IProjectRetrievalService
{
    public async Task<Result<Project>> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        return await projectRepository.GetByIdAsync(id, cancellationToken);
    }
}
