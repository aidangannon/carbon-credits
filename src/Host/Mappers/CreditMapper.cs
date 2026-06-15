using Core.Models;
using Host.Models;

namespace Host.Mappers;

public static class CreditMapper
{
    public static CreditResponse ToResponse(this Credit credit)
    {
        return new CreditResponse
        {
            Id = credit.Id,
            IssuedAt = credit.IssuedAt,
            ProjectId = credit.ProjectId,
            RetiredAt = credit.RetiredAt
        };
    }
}
