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
    Task<Result<T>> GetAsync<T>(string path, CancellationToken cancellationToken) where T : class;
    void Add(string path, object value);
    Task<Result> SaveAsync(CancellationToken cancellationToken);
}

public class FileStore : IFileStore
{
    /// <summary>
    /// Tracks a pending write for a path: the live object callers may mutate in place, and the
    /// etag that was on disk at the time it was last read (null if the path was never read in
    /// this unit of work, e.g. a fresh Add), used purely as the optimistic-concurrency check.
    /// </summary>
    private sealed record PendingChange(object Value, string? ExpectedEtag);

    private readonly RepositoryLock _lock = new();
    private readonly ConcurrentDictionary<string, PendingChange> _changes = new();

    public async Task<Result<T>> GetAsync<T>(string path, CancellationToken cancellationToken) where T : class
    {
        if (!File.Exists(path))
        {
            return Result<T>.Err($"File record '{path}' not found");
        }

        var recordText = await File.ReadAllTextAsync(path, cancellationToken);
        var record = JsonSerializer.Deserialize<FileRecord>(recordText);

        // Value deserializes as a JsonElement (its static type is `object`), so it must be
        // converted to T explicitly rather than cast directly.
        var value = record!.Value is JsonElement element
            ? element.Deserialize<T>()
            : record.Value as T;

        if (value is null)
        {
            throw new InvalidCastException($"Cannot cast file store value '{typeof(T).Name}' from {Environment.NewLine}{recordText}");
        }

        // Track the live typed instance (not the raw disk snapshot) so that in-place mutations
        // callers make to the returned aggregate are what actually get persisted on SaveAsync.
        _changes[path] = new PendingChange(value, record.Meta.Etag);

        return Result<T>.Ok(value);
    }

    public void Add(string path, object value)
    {
        _changes.TryGetValue(path, out var existing);

        // Preserve the expected etag from a prior GetAsync on this path (if any) so the
        // concurrency check still applies; a path added without ever being read is a fresh
        // write with nothing to conflict against.
        _changes[path] = new PendingChange(value, existing?.ExpectedEtag);
    }

    /// <summary>Locks the record's partition, validates the tracked change against the current etag and persists the value.</summary>
    public async Task<Result> SaveAsync(CancellationToken cancellationToken)
    {
        var results = ResultCollection.Empty();

        foreach(var change in _changes)
        {
            results.Add(await SaveAsync(change.Key, cancellationToken));
        }

        return results.ToResult();
    }

    private async Task<Result> SaveAsync(string path, CancellationToken cancellationToken)
    {
        await using var partitionLock = await _lock.AcquireAsync(path, cancellationToken);

        _changes.TryGetValue(path, out var change);

        var currentEtag = File.Exists(path)
            ? JsonSerializer.Deserialize<FileRecord>(await File.ReadAllTextAsync(path, cancellationToken))!.Meta.Etag
            : null;

        // Only enforce the conflict check when this path was actually read earlier in this
        // unit of work; a path that was only Add()-ed has nothing to conflict against.
        if (change!.ExpectedEtag is not null && change.ExpectedEtag != currentEtag)
        {
            throw new InvalidOperationException($"Conflict in file '{path}', tracked etag '{change.ExpectedEtag}' does not match current record etag '{currentEtag}'");
        }

        var record = new FileRecord
        {
            Value = change.Value,
            Meta = new MetaRecord
            {
                Etag = CreateEtag(change.Value)
            }
        };

        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(record), cancellationToken);

        _changes.TryRemove(path, out _);

        return Result.Ok();
    }

    private static string CreateEtag(object value)
    {
        var payload = JsonSerializer.Serialize(value);

        // collision extremely unlikely 16 chars in more than enough for basic etag
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        var etag = Convert.ToHexString(hash)[..16];

        return etag;
    }
}
