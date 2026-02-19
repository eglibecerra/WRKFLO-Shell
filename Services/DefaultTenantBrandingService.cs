using MudBlazor;
using Wrkflo.Shell.Models;

namespace Wrkflo.Shell.Services;

/// <summary>
/// Returns default WRKFLO branding. Replace with a DB-backed implementation per tenant.
/// </summary>
public class DefaultTenantBrandingService : ITenantBrandingService
{
    public Task<TenantBranding> GetBrandingAsync()
    {
        return Task.FromResult(new TenantBranding());
    }

    public string GenerateCssVariables(TenantBranding b)
    {
        var font = string.IsNullOrEmpty(b.FontFamily)
            ? "'Roboto', 'Helvetica', 'Arial', sans-serif"
            : b.FontFamily;

        return $@":root {{
    --wrkflo-primary: {b.Primary};
    --wrkflo-primary-light: {b.PrimaryLight};
    --wrkflo-background: {b.Background};
    --wrkflo-surface: {b.Surface};
    --wrkflo-text-primary: {b.TextPrimary};
    --wrkflo-text-secondary: {b.TextSecondary};
    --wrkflo-border: {b.Border};
    --wrkflo-success: {b.Success};
    --wrkflo-warning: {b.Warning};
    --wrkflo-error: {b.Error};
    --wrkflo-info: {b.Info};
    --wrkflo-font-family: {font};
    --wrkflo-border-radius: 8px;
}}";
    }

    public MudTheme GenerateMudTheme(TenantBranding b)
    {
        var fontFamily = string.IsNullOrEmpty(b.FontFamily)
            ? new[] { "Roboto", "Helvetica", "Arial", "sans-serif" }
            : b.FontFamily.Split(',').Select(f => f.Trim().Trim('\'')).ToArray();

        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = b.Primary,
                PrimaryLighten = b.PrimaryLight,
                Secondary = b.PrimaryLight,
                Background = b.Background,
                Surface = b.Surface,
                TextPrimary = b.TextPrimary,
                TextSecondary = b.TextSecondary,
                AppbarBackground = b.Background,
                AppbarText = b.Primary,
                DrawerBackground = b.Surface,
                DrawerText = b.TextPrimary,
                Success = b.Success,
                Warning = b.Warning,
                Error = b.Error,
                Info = b.Info
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = fontFamily
                }
            }
        };
    }
}
