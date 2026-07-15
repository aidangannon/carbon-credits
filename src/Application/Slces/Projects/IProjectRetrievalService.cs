using Core.Models;
using Crosscutting.Result;

namespace Application.Slces.Projects;

public interface IProjectRetrievalService
{
    Task<Result<Project>> GetProjectById(Guid id, CancellationToken cancellationToken);
}
