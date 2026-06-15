using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public interface IAccountRetrievalService
{
    Task<Result<Account>> GetAccountById(Guid id);
}
