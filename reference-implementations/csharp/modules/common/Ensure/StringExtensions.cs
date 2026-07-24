using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDo.Common.Ensure;

public static class StringExtensions
{
    /// <summary>Throws if <paramref name="value"/> is null or "".</summary>
    public static string EnsureNonEmpty(this string? value,
        [CallerArgumentExpression(nameof(value))] string paramName = null!)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Must not be null or empty.", paramName);
        }

        return value;
    }

    /// <summary>Throws if <paramref name="value"/> is null, "", or whitespace-only.</summary>
    public static string EnsureNonBlank(this string? value,
        [CallerArgumentExpression(nameof(value))] string paramName = null!)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Must not be null, blank, or empty.", paramName);
        }

        return value;
    }
}