using System.Runtime.CompilerServices;

namespace ToDo.Common.Ensure;

public static class GuidExtensions
{
    /// <summary>Throws if <paramref name="value"/> equals <see cref="Guid.Empty"/>.</summary>
    public static Guid EnsureNonEmpty(this Guid value,
        [CallerArgumentExpression(nameof(value))] string paramName = null!)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Must not be empty.", paramName);
        }

        return value;
    }
}