using Core.Models;
using Host.Models;

namespace Host.Mappers;

public static class ProjectMapper
{
    public static ProjectResponse ToResponse(this Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Country = project.Country,
            Type = project.Type.ToString()
        };
    }

    public static Project ToProject(this CreateProjectRequest request)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Country = request.Country,
            Type = Enum.Parse<ProjectType>(request.Type)
        };
    }
}
