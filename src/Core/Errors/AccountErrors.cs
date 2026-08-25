namespace Core.Errors;

public static class AccountErrors
{
    public const string NotFound = $"{nameof(AccountErrors)}.{nameof(NotFound)}";
    public const string CreatedInFuture = $"{nameof(AccountErrors)}.{nameof(CreatedInFuture)}";
}
