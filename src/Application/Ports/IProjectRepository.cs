using Core;

namespace Application.Ports;

public interface IProjectRepository
{
    Task CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task<Project> GetByIdAsync(Guid id);
}
