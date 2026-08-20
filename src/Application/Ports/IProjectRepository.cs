using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IProjectRepository
{
    Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> SaveAsync(Project project, CancellationToken cancellationToken);
}
