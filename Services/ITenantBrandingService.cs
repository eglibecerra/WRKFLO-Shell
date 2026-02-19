using Wrkflo.Shell.Models;

namespace Wrkflo.Shell.Services;

/// <summary>
/// Provides tenant branding configuration.
/// Swap the implementation to read from the database when ready.
/// </summary>
public interface ITenantBrandingService
{
    /// <summary>
    /// Get branding for the current tenant.
    /// </summary>
    Task<TenantBranding> GetBrandingAsync();

    /// <summary>
    /// Generate CSS custom properties from the branding config.
    /// </summary>
    string GenerateCssVariables(TenantBranding branding);

    /// <summary>
    /// Generate a MudTheme from the branding config to keep MudBlazor in sync.
    /// </summary>
    MudBlazor.MudTheme GenerateMudTheme(TenantBranding branding);
}
