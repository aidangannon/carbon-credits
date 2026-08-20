using System.Collections.Concurrent;

namespace Persistence.Locking;

/// <summary>Per-key async mutual exclusion for repositories.</summary>
public class RepositoryLock
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<IAsyncDisposable> AcquireAsync(string partition, CancellationToken cancellationToken)
    {
        var semaphore = _locks.GetOrAdd(partition, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        return new WriteLock(semaphore);
    }

    private sealed class WriteLock(SemaphoreSlim semaphore) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            semaphore.Release();
            return ValueTask.CompletedTask;
        }
    }
}
