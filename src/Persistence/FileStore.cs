using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Crosscutting.Result;
using Persistence.Locking;
using Persistence.Models;

namespace Persistence;

/// <summary>Generic file-backed store handling record loading, optimistic-concurrency updates, change tracking and locking.</summary>
public interface IFileStore
{
    Task<Result<T>> GetAsync<T>(string path, CancellationToken cancellationToken);
    Task<Result> SaveAsync<T>(string path, T value, CancellationToken cancellationToken);
}

public class FileStore : IFileStore
{
    private readonly RepositoryLock _lock = new();
    private readonly ConcurrentDictionary<string, MetaRecord> _changes = new();

    public async Task<Result<T>> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return Result<T>.Err($"File record '{path}' not found");
        }

        var recordText = await File.ReadAllTextAsync(path, cancellationToken);
        var record = JsonSerializer.Deserialize<FileRecord<T>>(recordText);

        _changes[path] = record!.Meta;

        return Result<T>.Ok(record.Value);
    }

    /// <summary>Locks the record's partition, validates the tracked change against the current etag and persists the value.</summary>
    public async Task<Result> SaveAsync<T>(string path, T value, CancellationToken cancellationToken)
    {
        await using var partitionLock = await _lock.AcquireAsync(path, cancellationToken);

        _changes.TryGetValue(path, out var trackedMeta);

        var currentEtag = File.Exists(path)
            ? JsonSerializer.Deserialize<FileRecord<T>>(await File.ReadAllTextAsync(path, cancellationToken))!.Meta.Etag
            : null;

        if (currentEtag is not null && trackedMeta?.Etag != currentEtag)
        {
            throw new InvalidOperationException($"Conflict in file '{path}', tracked etag '{trackedMeta?.Etag}' does not match current record etag '{currentEtag}'");
        }

        var payload = JsonSerializer.Serialize(value);

        // collision extremely unlikely 16 chars in more than enough for basic etag
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        var etag = Convert.ToHexString(hash)[..16];

        var record = new FileRecord<T> { Value = value, Meta = new MetaRecord { Etag = etag } };
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(record), cancellationToken);

        _changes.TryRemove(path, out _);

        return Result.Ok();
    }
}
