using Host.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Host.Swagger;

public class AccountResponseExample : IExamplesProvider<AccountResponse>
{
    public AccountResponse GetExamples()
    {
        return new AccountResponse
        {
            Id = Guid.NewGuid(),
            Name = "Test Account",
            CreatedAt = DateTime.Now,
            Credits = [
                new CreditResponse
                {
                    Id = Guid.NewGuid(),
                    IssuedAt = DateTime.Now - TimeSpan.FromDays(5),
                    ProjectId = Guid.NewGuid(),
                    RetiredAt = null,
                },
                new CreditResponse
                {
                    Id = Guid.NewGuid(),
                    IssuedAt = DateTime.Now - TimeSpan.FromDays(3),
                    ProjectId = Guid.NewGuid(),
                    RetiredAt = null,
                },
                new CreditResponse
                {
                    Id = Guid.NewGuid(),
                    IssuedAt = DateTime.Now - TimeSpan.FromDays(2),
                    ProjectId = Guid.NewGuid(),
                    RetiredAt = null,
                }
            ]
        };
    }
}
