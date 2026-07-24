using System.Runtime.CompilerServices;

namespace ToDo.Common.Ensure;

public static class DateTimeExtensions
{
    /// <summary>
    /// Throws if <paramref name="value"/> equals <c>default</c>. Catches unset fields/properties,
    /// including ones that skip constructor/initializer assignment (e.g. some deserializers).
    /// Note: <c>default(DateTime)</c> equals <see cref="DateTime.MinValue"/>, so a value deliberately
    /// set to <see cref="DateTime.MinValue"/> is indistinguishable from one that was never set.
    /// </summary>
    public static DateTime EnsureNonEmpty(this DateTime value,
        [CallerArgumentExpression(nameof(value))] string paramName = null!)
    {
        if (value == default)
        {
            throw new ArgumentException("Must not be empty.", paramName);
        }

        return value;
    }
}
