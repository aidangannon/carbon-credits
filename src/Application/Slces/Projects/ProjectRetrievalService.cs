using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slces.Projects;

public class ProjectRetrievalService(IProjectRepository projectRepository) : IProjectRetrievalService
{
    public async Task<Result<Project>> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        return await projectRepository.GetByIdAsync(id, cancellationToken);
    }
}
