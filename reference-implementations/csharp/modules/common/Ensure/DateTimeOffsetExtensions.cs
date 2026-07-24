using System.Runtime.CompilerServices;

namespace ToDo.Common.Ensure;

public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// Throws if <paramref name="value"/> equals <c>default</c>. Catches unset fields/properties,
    /// including ones that skip constructor/initializer assignment (e.g. some deserializers).
    /// Note: <c>default(DateTimeOffset)</c> equals <see cref="DateTimeOffset.MinValue"/>, so a value
    /// deliberately set to <see cref="DateTimeOffset.MinValue"/> is indistinguishable from one that was never set.
    /// </summary>
    public static DateTimeOffset EnsureNonEmpty(this DateTimeOffset value,
        [CallerArgumentExpression(nameof(value))] string paramName = null!)
    {
        if (value == default)
        {
            throw new ArgumentException("Must not be empty.", paramName);
        }

        return value;
    }
}
