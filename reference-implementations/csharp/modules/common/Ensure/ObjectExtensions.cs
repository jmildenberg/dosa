using System.Runtime.CompilerServices;

namespace ToDo.Common.Ensure;

public static class ObjectExtensions
{
    /// <summary>Throws if <paramref name="nullableInput"/> is null.</summary>
    public static T EnsureNonNull<T>(this T? nullableInput,
        [CallerArgumentExpression(nameof(nullableInput))]
        string paramName = null!) where T : class
    {
        if (nullableInput is null)
        {
            throw new ArgumentException("Must not be null.", paramName);
        }

        return nullableInput;
    }

    /// <summary>Throws if <paramref name="value"/> is null. Does not check for default/empty values of <typeparamref name="T"/>.</summary>
    public static T EnsureNonNull<T>(this T? value,
        [CallerArgumentExpression(nameof(value))]
        string paramName = null!) where T : struct
    {
        if (!value.HasValue)
        {
            throw new ArgumentException("Must not be null.", paramName);
        }

        return value.Value;
    }
}
  