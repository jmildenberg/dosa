namespace ToDo.Common.Tests.Ensure;

using ToDo.Common.Ensure;

public sealed class ObjectExtensionsTests
{
    [Fact]
    public void EnsureNonNull_ThrowsArgumentException_WhenNull()
    {
        string? nullString = null;
        int? nullInt = null;
        long? nullLng = null;
        float? nullFloat = null;
        double? nullDouble = null;
        bool? nullBool = null;
        object? nullObject = null;
        Guid? nullGuid = null;
        DateTime? nullDateTime = null;
        DateTimeOffset? nullDateTimeOffset = null;

        Assert.Multiple(
            () => Should.Throw<ArgumentException>(() => nullString.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullString)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullString)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullInt.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullInt)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullInt)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullLng.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullLng)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullLng)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullFloat.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullFloat)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullFloat)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullDouble.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullDouble)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullDouble)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullBool.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullBool)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullBool)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullObject.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullObject)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullObject)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullGuid.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullGuid)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullGuid)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullDateTime.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullDateTime)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullDateTime)}')")
                ),
            () => Should.Throw<ArgumentException>(() => nullDateTimeOffset.EnsureNonNull())
                .ShouldSatisfyAllConditions(
                    e => e.ParamName.ShouldBe(nameof(nullDateTimeOffset)),
                    e => e.Message.ShouldBe($"Must not be null. (Parameter '{nameof(nullDateTimeOffset)}')")
                )
        );
    }

    [Fact]
    public void EnsureNonNull_ReturnsNonNullableType_WhenNotNull()
    {
        string? nullString = "";
        int? nullInt = 1;
        long? nullLng = 2;
        float? nullFloat = 3;
        double? nullDouble = 4;
        bool? nullBool = true;
        object? nullObject = new();
        Guid? nullGuid = Guid.NewGuid();
        DateTime? nullDateTime = DateTime.UtcNow;
        DateTimeOffset? nullDateTimeOffset = DateTimeOffset.UtcNow;

        Assert.Multiple(
            () => nullString.EnsureNonNull().ShouldBe(nullString),
            () => nullInt.EnsureNonNull().ShouldBe(nullInt.Value),
            () => nullLng.EnsureNonNull().ShouldBe(nullLng.Value),
            () => nullFloat.EnsureNonNull().ShouldBe(nullFloat.Value),
            () => nullDouble.EnsureNonNull().ShouldBe(nullDouble.Value),
            () => nullBool.EnsureNonNull().ShouldBe(nullBool.Value),
            () => nullObject.EnsureNonNull().ShouldBe(nullObject),
            () => nullGuid.EnsureNonNull().ShouldBe(nullGuid.Value),
            () => nullDateTime.EnsureNonNull().ShouldBe(nullDateTime.Value),
            () => nullDateTimeOffset.EnsureNonNull().ShouldBe(nullDateTimeOffset.Value)
        );
    }
}