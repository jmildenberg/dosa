using System.Collections.Immutable;

namespace ToDo.Common;

using Ensure;
using Exceptions;

/// <summary>
/// Represents the outcome of an operation that can fail without throwing: either a value, or a
/// status and the <see cref="Problem"/>s that caused the failure.
/// </summary>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly IReadOnlySet<Problem>? _problems;

    public bool IsSuccess { get; }
    public int? Status { get; }
    public IReadOnlySet<Problem> Problems => _problems ?? ImmutableHashSet<Problem>.Empty;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        Status = null;
        _problems = ImmutableHashSet<Problem>.Empty;
    }

    private Result(int status, IEnumerable<Problem> problems)
    {
        IsSuccess = false;
        _value = default;
        Status = status;
        _problems = new HashSet<Problem>(problems.EnsureNonNull(), Problem.Comparer);
    }

    internal static Result<T> Success(T value) => new(value);

    internal static Result<T> Failure(int status) => new(status, []);

    internal static Result<T> Failure(int status, Problem problem) => new(status, [problem.EnsureNonNull()]);

    internal static Result<T> Failure(int status, IEnumerable<Problem> problems) => new(status, problems);
}

/// <summary>
/// Generic-method entry points for <see cref="Result{T}"/> that allow <c>T</c> to be inferred where
/// possible (e.g. <c>Result.Success(value)</c>), instead of always requiring <c>Result&lt;T&gt;.Success(value)</c>.
/// </summary>
public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(int status) => Result<T>.Failure(status);

    public static Result<T> Failure<T>(int status, Problem problem) => Result<T>.Failure(status, problem);

    public static Result<T> Failure<T>(int status, IEnumerable<Problem> problems) => Result<T>.Failure(status, problems);
}

/// <summary>
/// A type with exactly one value, used as the <c>T</c> of <see cref="Result{T}"/> for operations
/// that can fail but have no meaningful value to return on success.
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value;
}
