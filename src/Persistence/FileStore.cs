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
    private readonly RepositoryLock _lock = new();
    private readonly ConcurrentDictionary<string, FileRecord> _changes = new();

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

        _changes[path] = new FileRecord { Value = value, Meta = record.Meta };

        return Result<T>.Ok(value);
    }

    public void Add(string path, object value)
    {
        _changes.TryGetValue(path, out var existing);

        _changes[path] = new FileRecord
        {
            Value = value,
            Meta = existing?.Meta ?? new MetaRecord { Etag = CreateEtag(value) }
        };
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

        _changes.TryGetValue(path, out var record);

        var currentEtag = File.Exists(path)
            ? JsonSerializer.Deserialize<FileRecord>(await File.ReadAllTextAsync(path, cancellationToken))!.Meta.Etag
            : null;

        if (currentEtag is not null && record?.Meta.Etag != currentEtag)
        {
            throw new InvalidOperationException($"Conflict in file '{path}', tracked etag '{record?.Meta.Etag}' does not match current record etag '{currentEtag}'");
        }

        record!.Meta.Etag = CreateEtag(record.Value);

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
