namespace ToDo.Common.Exceptions;

using Ensure;

/// <summary>
/// The core application runtime exception used across layers to provide 
/// structured details about request or operation failures.
/// </summary>
public class AppRuntimeException : Exception
{
    
    private readonly HashSet<Problem> _problems = new(Problem.Comparer);
    
    /// <summary>
    /// Gets the target integer status code this exception should represent.
    /// </summary>
    /// <remarks>the integer status code is a superset of http status codes</remarks>
    public int Status { get; }
    
    /// <summary>
    /// Gets a read-only collection of unique problems that caused this exception.
    /// </summary>
    public IReadOnlySet<Problem> Problems => _problems;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntimeException"/> class.
    /// </summary>
    /// <param name="status">The integers status code this exception represents.</param>
    /// <param name="message">The detailed message (primarily used for internal system logging).</param>
    /// <param name="innerException">The underlying cause of the exception.</param>
    public AppRuntimeException(int status, string? message, Exception? innerException = null)
    : base(message, innerException)
    {
        Status = status;
    }
    
    /// <summary>
    /// Adds a single structured problem to the exception context.
    /// </summary>
    /// <param name="problem">The problem detail definition.</param>
    /// <returns>The current exception instance to support fluent method chaining.</returns>
    public AppRuntimeException WithProblem(Problem problem)
    {
        _problems.Add(problem.EnsureNonNull());
        return this;
    }
    
    /// <summary>
    /// Adds a collection of structured problems to the exception context.
    /// </summary>
    /// <param name="problems">A collection of problem details.</param>
    /// <returns>The current exception instance to support fluent method chaining.</returns>
    public AppRuntimeException WithProblems(IEnumerable<Problem> problems)
    {
        foreach (var problem in problems.EnsureNonNull())
        {
            _problems.Add(problem.EnsureNonNull());
        }
        
        return this;
    }
    
}