namespace Wrkflo.Shell.Models;

/// <summary>
/// Represents a tenant's branding configuration.
/// Loaded from the database per-tenant on login.
/// </summary>
public class TenantBranding
{
    /// <summary>Tenant identifier.</summary>
    public string TenantId { get; set; } = "";

    /// <summary>Tenant display name (shown in nav if no logo).</summary>
    public string TenantName { get; set; } = "WrkFlo";

    /// <summary>URL to the tenant's logo image. Null = use default text logo.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Logo alt text for accessibility.</summary>
    public string? LogoAlt { get; set; }

    /// <summary>"Powered by" badge visibility. Always true — cannot be disabled.</summary>
    public bool ShowPoweredBy { get; set; } = true;

    // ===== Colour Palette =====

    /// <summary>Primary brand colour (e.g. #4F46E5).</summary>
    public string Primary { get; set; } = "#4F46E5";

    /// <summary>Primary light variant (e.g. #818CF8).</summary>
    public string PrimaryLight { get; set; } = "#818CF8";

    /// <summary>Background colour (e.g. #F9FAFB).</summary>
    public string Background { get; set; } = "#F9FAFB";

    /// <summary>Surface colour for cards/papers (e.g. #FFFFFF).</summary>
    public string Surface { get; set; } = "#FFFFFF";

    /// <summary>Primary text colour (e.g. #111827).</summary>
    public string TextPrimary { get; set; } = "#111827";

    /// <summary>Secondary text colour (e.g. #6B7280).</summary>
    public string TextSecondary { get; set; } = "#6B7280";

    /// <summary>Border colour (e.g. #E5E7EB).</summary>
    public string Border { get; set; } = "#E5E7EB";

    /// <summary>Success colour.</summary>
    public string Success { get; set; } = "#10B981";

    /// <summary>Warning colour.</summary>
    public string Warning { get; set; } = "#F59E0B";

    /// <summary>Error colour.</summary>
    public string Error { get; set; } = "#EF4444";

    /// <summary>Info colour.</summary>
    public string Info { get; set; } = "#3B82F6";

    // ===== Typography =====

    /// <summary>Font family stack. Null = use system default.</summary>
    public string? FontFamily { get; set; }

    // ===== Enterprise Features =====

    /// <summary>Custom CSS injected after all other styles. Enterprise tier only.</summary>
    public string? CustomCss { get; set; }

    /// <summary>Plan tier controlling which branding features are available.</summary>
    public PlanTier Plan { get; set; } = PlanTier.Starter;
}

public enum PlanTier
{
    /// <summary>Default WRKFLO branding only.</summary>
    Starter,

    /// <summary>Logo upload + colour palette customisation.</summary>
    Pro,

    /// <summary>Full custom CSS + font upload.</summary>
    Enterprise
}
