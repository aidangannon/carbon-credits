using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public interface IProjectCreationService
{
    Task<Result<Project>> CreateProject(Project project, CancellationToken cancellationToken);
}
