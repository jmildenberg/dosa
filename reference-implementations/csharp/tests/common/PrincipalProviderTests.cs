using System.Security.Claims;

using ToDo.Common;

namespace ToDo.Common.Tests;

public sealed class PrincipalProviderTests
{
    [Fact]
    public void System_Principal_IsAuthenticated()
    {
        PrincipalProvider.System.Principal.Identity!.IsAuthenticated.ShouldBeTrue();
    }

    [Fact]
    public void System_Principal_HasSystemNameIdentifier()
    {
        PrincipalProvider.System.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value.ShouldBe("system");
    }

    [Fact]
    public void System_ReturnsSameInstance()
    {
        PrincipalProvider.System.ShouldBeSameAs(PrincipalProvider.System);
    }

    private sealed class FixedPrincipalProvider(ClaimsPrincipal principal) : PrincipalProvider
    {
        public override ClaimsPrincipal Principal { get; } = principal;
    }

    [Fact]
    public void Subclass_CanOverride_Principal()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-1")], "Test");
        var principal = new ClaimsPrincipal(identity);
        PrincipalProvider provider = new FixedPrincipalProvider(principal);

        provider.Principal.ShouldBeSameAs(principal);
    }
}
