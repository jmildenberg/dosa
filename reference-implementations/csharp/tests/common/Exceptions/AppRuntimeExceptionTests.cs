using ToDo.Common.Exceptions;

namespace ToDo.Common.Tests.Exceptions;

public sealed class AppRuntimeExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusMessageAndInnerException()
    {
        var inner = new InvalidOperationException("boom");
        var exception = new AppRuntimeException(400, "bad request", inner);

        Assert.Multiple(
            () => exception.Status.ShouldBe(400),
            () => exception.Message.ShouldBe("bad request"),
            () => exception.InnerException.ShouldBe(inner)
        );
    }

    [Fact]
    public void Problems_IsEmpty_WhenNoneAdded()
    {
        var exception = new AppRuntimeException(400, "bad request");

        exception.Problems.ShouldBeEmpty();
    }

    [Fact]
    public void WithProblem_AddsProblem_AndReturnsSameInstance_ForChaining()
    {
        var exception = new AppRuntimeException(400, "bad request");
        var problem = Problem.Of("NotBlank", "body", "name");

        var result = exception.WithProblem(problem);

        Assert.Multiple(
            () => result.ShouldBeSameAs(exception),
            () => exception.Problems.ShouldContain(problem)
        );
    }

    [Fact]
    public void WithProblem_ThrowsArgumentException_WhenProblemIsNull()
    {
        var exception = new AppRuntimeException(400, "bad request");

        Should.Throw<ArgumentException>(() => exception.WithProblem(null!));
    }

    [Fact]
    public void WithProblems_AddsAllProblems()
    {
        var exception = new AppRuntimeException(400, "bad request");
        var problems = new[]
        {
            Problem.Of("NotBlank", "body", "name"),
            Problem.Of("NotBlank", "body", "email"),
        };

        exception.WithProblems(problems);

        exception.Problems.Count.ShouldBe(2);
    }

    [Fact]
    public void WithProblems_ThrowsArgumentException_WhenCollectionIsNull()
    {
        var exception = new AppRuntimeException(400, "bad request");

        Should.Throw<ArgumentException>(() => exception.WithProblems(null!));
    }

    [Fact]
    public void WithProblems_ThrowsArgumentException_WhenAnyProblemIsNull()
    {
        var exception = new AppRuntimeException(400, "bad request");

        Should.Throw<ArgumentException>(() => exception.WithProblems([null!]));
    }

    [Fact]
    public void Problems_Deduplicates_ByCodeLocationTarget()
    {
        var exception = new AppRuntimeException(400, "bad request");

        exception
            .WithProblem(Problem.Of("NotBlank", "body", "name", "first"))
            .WithProblem(Problem.Of("NotBlank", "body", "name", "second"));

        exception.Problems.Count.ShouldBe(1);
    }
}
