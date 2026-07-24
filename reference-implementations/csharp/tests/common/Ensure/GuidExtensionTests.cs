using ToDo.Common.Ensure;

namespace ToDo.Common.Tests.Ensure;

public sealed class GuidExtensionTests
{
    [Fact]
    public void EnsureNotEmpty_ThrowsArgumentException_WhenEmpty()
    {
        Guid parameter = Guid.Empty;
        Should.Throw<ArgumentException>(() => parameter.EnsureNonEmpty())
            .ShouldSatisfyAllConditions(
                e => e.ParamName.ShouldBe(nameof(parameter)),
                e => e.Message.ShouldBe($"Must not be empty. (Parameter '{nameof(parameter)}')")
            );
    }

    [Fact]
    public void EnsureNotEmpty_ReturnsValue_WhenNotEmpty()
    {
        Guid value = Guid.NewGuid();
        Assert.Multiple(
            () => Should.NotThrow(() => value.EnsureNonEmpty()),
            () => value.EnsureNonEmpty().ShouldBe(value)
        );
    }

}