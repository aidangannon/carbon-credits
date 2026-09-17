namespace Core.Errors;

public static class CreditErrors
{
    public const string CannotCreateRetired = $"{nameof(CreditErrors)}.{nameof(CannotCreateRetired)}";
    public const string IssuedInFuture = $"{nameof(CreditErrors)}.{nameof(IssuedInFuture)}";
    public const string ProjectMismatch = $"{nameof(CreditErrors)}.{nameof(ProjectMismatch)}";
    public const string NotFound = $"{nameof(CreditErrors)}.{nameof(NotFound)}";
    public const string AlreadyRetired = $"{nameof(CreditErrors)}.{nameof(AlreadyRetired)}";
}
