using System.Security.Claims;

namespace ToDo.Common;

/// <summary>
/// Provides the <see cref="ClaimsPrincipal"/> for the current operation. Mirrors the shape of
/// <see cref="System.TimeProvider"/>: an abstract class with a built-in <see cref="System"/>
/// default, real implementations (e.g. HTTP-context-backed) override <see cref="Principal"/>.
/// </summary>
public abstract class PrincipalProvider
{
    /// <summary>A provider for operations with no real caller (e.g. background jobs, internal calls).</summary>
    public static PrincipalProvider System { get; } = new SystemPrincipalProvider();

    public abstract ClaimsPrincipal Principal { get; }

    private sealed class SystemPrincipalProvider : PrincipalProvider
    {
        private static readonly ClaimsPrincipal SystemClaimsPrincipal = new(
            new ClaimsIdentity(
                claims:
                [
                    new Claim(ClaimTypes.NameIdentifier, "system"),
                    new Claim(ClaimTypes.Name, "System"),
                ],
                authenticationType: "System"));

        public override ClaimsPrincipal Principal => SystemClaimsPrincipal;
    }
}
