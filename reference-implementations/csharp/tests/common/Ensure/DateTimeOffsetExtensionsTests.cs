using ToDo.Common.Ensure;

namespace ToDo.Common.Tests.Ensure;

public sealed class DateTimeOffsetExtensionsTests
{
    [Fact]
    public void EnsureNonEmpty_ThrowsArgumentException_WhenDefault()
    {
        DateTimeOffset parameter = default;
        Should.Throw<ArgumentException>(() => parameter.EnsureNonEmpty())
            .ShouldSatisfyAllConditions(
                e => e.ParamName.ShouldBe(nameof(parameter)),
                e => e.Message.ShouldBe($"Must not be empty. (Parameter '{nameof(parameter)}')")
            );
    }

    [Fact]
    public void EnsureNonEmpty_ReturnsValue_WhenNotDefault()
    {
        DateTimeOffset value = DateTimeOffset.UtcNow;
        Assert.Multiple(
            () => Should.NotThrow(() => value.EnsureNonEmpty()),
            () => value.EnsureNonEmpty().ShouldBe(value)
        );
    }
}
