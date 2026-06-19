using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IProjectRepository
{
    Task<Result> CreateAsync(Project project, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
