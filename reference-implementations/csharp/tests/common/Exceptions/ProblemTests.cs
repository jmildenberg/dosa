using ToDo.Common.Exceptions;

namespace ToDo.Common.Tests.Exceptions;

public sealed class ProblemTests
{
    [Fact]
    public void Of_SetsAllProperties()
    {
        var arguments = new object[] { "a", 1 };
        var problem = Problem.Of("NotBlank", "body", "name", arguments);

        Assert.Multiple(
            () => problem.Code.ShouldBe("NotBlank"),
            () => problem.Location.ShouldBe("body"),
            () => problem.Target.ShouldBe("name"),
            () => problem.Arguments.ShouldBe(arguments)
        );
    }

    [Fact]
    public void Of_TargetDefaultsToNull_AndArgumentsDefaultsToEmpty()
    {
        var problem = Problem.Of("NotBlank", "body");

        Assert.Multiple(
            () => problem.Target.ShouldBeNull(),
            () => problem.Arguments.ShouldBeEmpty()
        );
    }

    [Theory]
    [InlineData(null, "body")]
    [InlineData("NotBlank", null)]
    public void Of_ThrowsArgumentException_WhenCodeOrLocationIsNull(string? code, string? location)
    {
        Should.Throw<ArgumentException>(() => Problem.Of(code!, location!));
    }

    [Fact]
    public void Comparer_TreatsProblems_AsEqual_WhenCodeLocationTargetMatch()
    {
        var first = Problem.Of("NotBlank", "body", "name", "unused");
        var second = Problem.Of("NotBlank", "body", "name", "different", "args");

        Assert.Multiple(
            () => Problem.Comparer.Equals(first, second).ShouldBeTrue(),
            () => Problem.Comparer.GetHashCode(first).ShouldBe(Problem.Comparer.GetHashCode(second))
        );
    }

    [Theory]
    [InlineData("Different", "body", "name")]
    [InlineData("NotBlank", "query", "name")]
    [InlineData("NotBlank", "body", "other")]
    public void Comparer_TreatsProblems_AsNotEqual_WhenCodeLocationOrTargetDiffer(
        string code, string location, string? target)
    {
        var reference = Problem.Of("NotBlank", "body", "name");
        var other = Problem.Of(code, location, target);

        Problem.Comparer.Equals(reference, other).ShouldBeFalse();
    }
}
