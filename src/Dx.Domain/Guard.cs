namespace Dx.Domain;

public static partial class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> AgainstNull<T>(T? value, DomainError error)
        where T : class
        => value is null ? Result<T>.Failure(error) : Result<T>.Ok(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<string> AgainstNullOrWhiteSpace(string? value, DomainError error)
        => string.IsNullOrWhiteSpace(value)
            ? Result<string>.Failure(error)
            : Result<string>.Ok(value!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> AgainstDefault<T>(T value, DomainError error)
        where T : struct
        => EqualityComparer<T>.Default.Equals(value, default)
            ? Result<T>.Failure(error)
            : Result<T>.Ok(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> AgainstOutOfRange<T>(
        T value,
        T minInclusive,
        T maxInclusive,
        DomainError error)
        where T : IComparable<T>
        => value.CompareTo(minInclusive) < 0 || value.CompareTo(maxInclusive) > 0
            ? Result<T>.Failure(error)
            : Result<T>.Ok(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<Guid> AgainstEmptyGuid(Guid value, DomainError error)
        => value == Guid.Empty
            ? Result<Guid>.Failure(error)
            : Result<Guid>.Ok(value);
}
