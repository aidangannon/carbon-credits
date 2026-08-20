using Application.Ports;
using Core.Errors;
using Core.Models;
using Crosscutting.Result;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Persistence.Adapters;

public class FileAccountRepository(IOptions<FileOptions> fileOptions, IFileStore fileStore) : IAccountRepository
{
    public async Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/accounts/{id}";

        var result = await fileStore.GetAsync<Account>(path, cancellationToken);

        return result.HasFailed()
            ? Result<Account>.Err(AccountErrors.NotFound)
            : Result<Account>.Ok(result.Unwrap());
    }

    public async Task<Result> SaveAsync(Account account, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/accounts/{account.Id}";

        return await fileStore.SaveAsync(path, account, cancellationToken);
    }
}
