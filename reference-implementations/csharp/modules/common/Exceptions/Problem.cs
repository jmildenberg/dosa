namespace ToDo.Common.Exceptions;
using Ensure;

/// <summary>
/// Represents a structured, machine-readable error condition encountered during application execution.
/// </summary>
public class Problem
{
    /// <summary>Compares <see cref="Problem"/> instances by Code, Location, and Target.</summary>
    public static IEqualityComparer<Problem> Comparer { get; } = new ProblemComparer();

    /// <summary>
    /// Gets a specific, stable alphanumeric identifier for the error type (e.g., "NotBlank", "InvalidFormat").
    /// </summary>
    public string Code { get; init; }
    /// <summary>
    /// Gets the architectural or payload layer where the problem was intercepted (e.g., "body", "query", "header").
    /// </summary>
    public string Location { get; init; }
    /// <summary>
    /// Gets the name of the specific property, field, or boundary component that triggered the failure. 
    /// Returns <see langword="null"/> if the error applies to the entire request context rather than a single field.
    /// </summary>
    public string? Target { get; init; }
    /// <summary>
    /// Gets an optional collection of contextual values used to format localized, human-readable error messages.
    /// </summary>
    public object[]? Arguments { get; init; }
    
    // Private Constructor enforces factory usage.
    private Problem(string code, string location, string? target, object[]? args)
    {
        Code = code;
        Location = location;
        Target = target;
        Arguments = args;
    }

    /// <summary>
    /// Creates a <see cref="Problem"/> with the provided details.
    /// </summary>
    /// <param name="code">The server defined error code.</param>
    /// <param name="location">The location where the problem occurred.</param>
    /// <param name="target">The name of the property that has caused the problem. (optional)</param>
    /// <param name="arguments">The arguments for formatting the message. (optional)</param>
    public static Problem Of(string code, string location, string? target = null, params object[]? arguments)
    {
        return new Problem(code.EnsureNonNull(),location.EnsureNonNull(), target, arguments);
    }
}