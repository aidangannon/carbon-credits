namespace Application.Ports;

public interface IProjectRepository
{
    Task CreateAsync();
    Task UpdateAsync();
    Task GetAllAsync();
    Task GetByIdAsync();
}
