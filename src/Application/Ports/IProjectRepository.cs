using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IProjectRepository
{
    Task<Result> CreateAsync(Project project);
    Task<Result> UpdateAsync(Project project);
    Task<Result<Project>> GetByIdAsync(Guid id);
}
