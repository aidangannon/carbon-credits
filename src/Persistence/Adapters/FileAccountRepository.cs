using System.Collections.Concurrent;
using System.Text.Json;
using Application.Ports;
using Core.Errors;
using Core.Models;
using Core.Result;
using Crosscutting.Result;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Persistence.Adapters;

public class FileAccountRepository(IOptions<FileOptions> fileOptions) : IAccountRepository
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    public async Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/accounts/{id}";

        if (!File.Exists(path))
        {
            return Result<Account>.Err(AccountErrors.NotFound);
        }

        var accountText = await File.ReadAllTextAsync(path, cancellationToken);
        var account = JsonSerializer.Deserialize<Account>(accountText);

        return Result<Account>.Ok(account!);
    }

    public async Task<Result> SaveAsync(Account account, CancellationToken cancellationToken)
    {
        var basePath = fileOptions.Value?.BasePath ?? throw new ArgumentNullException("BasePath", "File base path cannot be null");
        var path = $"{basePath}/accounts/{account.Id}";

        var accountText = JsonSerializer.Serialize(account);
        await File.WriteAllTextAsync(path, accountText, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Account>> UpdateAsync(Guid id, Func<Account, DomainResult> update, CancellationToken cancellationToken)
    {
        var semaphore = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var getResult = await GetByIdAsync(id, cancellationToken);
            if (getResult.HasFailed())
                return Result<Account>.Err(getResult.Error);

            var account = getResult.Unwrap();
            var domainResult = update(account);
            if (domainResult.HasFailed())
                return Result<Account>.Err(domainResult.Error);

            await SaveAsync(account, cancellationToken);
            return Result<Account>.Ok(account);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
