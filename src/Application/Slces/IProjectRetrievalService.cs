using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public interface IProjectRetrievalService
{
    Task<Result<Project>> GetProjectById(Guid id, CancellationToken cancellationToken);
}
