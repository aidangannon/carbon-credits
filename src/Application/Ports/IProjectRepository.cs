using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IProjectRepository
{
    Task<Result<Project>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Project project);
    Task<Result> SaveAsync(CancellationToken cancellationToken);
}
