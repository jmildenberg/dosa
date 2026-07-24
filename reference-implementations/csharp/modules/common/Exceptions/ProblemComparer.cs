namespace ToDo.Common.Exceptions;

/// <summary>
/// Compares <see cref="Problem"/> instances by <see cref="Problem.Code"/>, <see cref="Problem.Location"/>,
/// and <see cref="Problem.Target"/>. <see cref="Problem.Arguments"/> is excluded intentionally: the first
/// problem reported for a given Code/Location/Target wins, later ones with different Arguments are dropped.
/// Applies uniformly to subclasses of <see cref="Problem"/> — equality is by these three properties alone.
/// </summary>
internal sealed class ProblemComparer : IEqualityComparer<Problem>
{
    public bool Equals(Problem? x, Problem? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return (x.Code, x.Location, x.Target) == (y.Code, y.Location, y.Target);
    }

    public int GetHashCode(Problem obj) => HashCode.Combine(obj.Code, obj.Location, obj.Target);
}
