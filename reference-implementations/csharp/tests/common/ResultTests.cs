using ToDo.Common;
using ToDo.Common.Exceptions;

namespace ToDo.Common.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_IsSuccess_AndHasNoStatusOrProblems()
    {
        var result = Result<int>.Success(42);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Value.ShouldBe(42),
            () => result.Status.ShouldBeNull(),
            () => result.Problems.ShouldBeEmpty()
        );
    }

    [Fact]
    public void Failure_WithoutProblems_IsNotSuccess_AndCarriesStatusOnly()
    {
        var result = Result<int>.Failure(500);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.Status.ShouldBe(500),
            () => result.Problems.ShouldBeEmpty()
        );
    }

    [Fact]
    public void Failure_WithSingleProblem_IsNotSuccess_AndCarriesStatusAndProblem()
    {
        var problem = Problem.Of("NotBlank", "body", "title");
        var result = Result<int>.Failure(422, problem);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.Status.ShouldBe(422),
            () => result.Problems.ShouldContain(problem)
        );
    }

    [Fact]
    public void Failure_WithMultipleProblems_DeduplicatesByCodeLocationTarget()
    {
        var problems = new[]
        {
            Problem.Of("NotBlank", "body", "title", "first"),
            Problem.Of("NotBlank", "body", "title", "second"),
            Problem.Of("NotBlank", "body", "dueDate"),
        };

        var result = Result<int>.Failure(422, problems);

        result.Problems.Count.ShouldBe(2);
    }

    [Fact]
    public void Value_Throws_WhenResultIsFailure()
    {
        var result = Result<int>.Failure(422, Problem.Of("NotBlank", "body", "title"));

        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_ThrowsArgumentException_WhenProblemIsNull()
    {
        Should.Throw<ArgumentException>(() => Result<int>.Failure(422, (Problem)null!));
    }

    [Fact]
    public void Failure_ThrowsArgumentException_WhenProblemsCollectionIsNull()
    {
        Should.Throw<ArgumentException>(() => Result<int>.Failure(422, (IEnumerable<Problem>)null!));
    }
}

public sealed class ResultStaticHelperTests
{
    [Fact]
    public void Success_InfersT_AndReturnsSuccessfulResult()
    {
        var result = Result.Success(42);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Value.ShouldBe(42)
        );
    }

    [Fact]
    public void Failure_WithoutProblems_ReturnsFailureWithStatusOnly()
    {
        var result = Result.Failure<int>(500);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.Status.ShouldBe(500),
            () => result.Problems.ShouldBeEmpty()
        );
    }

    [Fact]
    public void Failure_WithSingleProblem_ReturnsFailureWithProblem()
    {
        var problem = Problem.Of("NotBlank", "body", "title");
        var result = Result.Failure<int>(422, problem);

        Assert.Multiple(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.Status.ShouldBe(422),
            () => result.Problems.ShouldContain(problem)
        );
    }

    [Fact]
    public void Failure_WithMultipleProblems_ReturnsFailureWithAllProblems()
    {
        var problems = new[]
        {
            Problem.Of("NotBlank", "body", "title"),
            Problem.Of("NotBlank", "body", "dueDate"),
        };

        var result = Result.Failure<int>(422, problems);

        result.Problems.Count.ShouldBe(2);
    }
}

public sealed class UnitTests
{
    [Fact]
    public void Value_EqualsDefault()
    {
        Unit.Value.ShouldBe(default(Unit));
    }

    [Fact]
    public void AllInstances_AreEqual()
    {
        var a = Unit.Value;
        var b = new Unit();

        a.ShouldBe(b);
    }
}
