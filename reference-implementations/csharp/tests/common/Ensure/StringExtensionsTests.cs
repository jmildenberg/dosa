using System.Runtime.InteropServices;

using Xunit.Sdk;

namespace ToDo.Common.Tests.Ensure;

using ToDo.Common.Ensure;

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EnsureNotEmpty_ThrowsArgumentException_WhenNullOrEmpty(string? parameter)
    {
        Should.Throw<ArgumentException>(() => parameter.EnsureNonEmpty())
            .ShouldSatisfyAllConditions(
                e => e.ParamName.ShouldBe(nameof(parameter)),
                e => e.Message.ShouldBe($"Must not be null or empty. (Parameter '{nameof(parameter)}')")
            );
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("NotBlank")]
    public void EnsureNotEmpty_ReturnsValue_WhenNotEmpty(string? value)
    {
        Assert.Multiple(
            () => Should.NotThrow(() => value.EnsureNonEmpty()),
            () => value.EnsureNonEmpty().ShouldBe(value)
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void EnsureNonBlank_ThrowsArgumentException_WhenNullEmptyOrBlank(string? parameter)
    {
        Should.Throw<ArgumentException>(() => parameter.EnsureNonBlank())
            .ShouldSatisfyAllConditions(
                e => e.ParamName.ShouldBe(nameof(parameter)),
                e => e.Message.ShouldBe($"Must not be null, blank, or empty. (Parameter '{nameof(parameter)}')")
            );
    }
    
    [Theory]
    [InlineData("NotBlank")]
    public void EnsureNonBlank_ReturnsValue_WhenNotBlank(string? value)
    {
        Assert.Multiple(
            () => Should.NotThrow(() => value.EnsureNonBlank()),
            () => value.EnsureNonEmpty().ShouldBe(value)
        );
    }
}