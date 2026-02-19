# Theming & Branding Guide

WRKFLO supports per-tenant branding through CSS Custom Properties (design tokens), a `ThemeProvider` component, and an `ITenantBrandingService`.

## Architecture

```
┌─────────────────────────────────────────────────┐
│  Database (TenantBranding table)                │
│  colours, logo URL, custom CSS, plan tier       │
└───────────────────┬─────────────────────────────┘
                    │
        ┌───────────▼───────────┐
        │ ITenantBrandingService │  ← swap implementation
        │ (DefaultTenantBranding │     for DB-backed service
        │  Service for now)      │
        └───────────┬───────────┘
                    │
        ┌───────────▼───────────┐
        │   ThemeProvider.razor  │  ← sits in MainLayout
        │                       │
        │  1. CSS variables     │  ← :root { --wrkflo-* }
        │  2. MudTheme sync     │  ← MudThemeProvider
        │  3. Custom CSS        │  ← Enterprise only
        └───────────────────────┘
                    │
        ┌───────────▼───────────┐
        │   All Components      │  ← consume var(--wrkflo-*)
        │   app.css             │     and MudBlazor Color.*
        └───────────────────────┘
```

## CSS Design Tokens

All tokens are defined as CSS Custom Properties on `:root`. Components reference them via `var(--wrkflo-*)`.

| Token | Default | Usage |
|---|---|---|
| `--wrkflo-primary` | #4F46E5 | Primary brand colour |
| `--wrkflo-primary-light` | #818CF8 | Lighter primary variant |
| `--wrkflo-background` | #F9FAFB | Page background |
| `--wrkflo-surface` | #FFFFFF | Card/paper backgrounds |
| `--wrkflo-text-primary` | #111827 | Main text colour |
| `--wrkflo-text-secondary` | #6B7280 | Secondary/muted text |
| `--wrkflo-border` | #E5E7EB | Border colour |
| `--wrkflo-success` | #10B981 | Success states |
| `--wrkflo-warning` | #F59E0B | Warning states |
| `--wrkflo-error` | #EF4444 | Error states |
| `--wrkflo-info` | #3B82F6 | Info states |
| `--wrkflo-font-family` | Roboto, ... | Font stack |
| `--wrkflo-border-radius` | 8px | Default border radius |

### Fallback Chain

1. `ThemeProvider.razor` injects tenant-specific `:root` values
2. `app.css` has fallback `:root` defaults (same values)
3. If neither loads, browser defaults apply

## TenantBranding Model

```csharp
public class TenantBranding
{
    public string TenantId { get; set; }
    public string TenantName { get; set; }      // Shown in nav if no logo
    public string? LogoUrl { get; set; }         // Pro+ only
    public string? LogoAlt { get; set; }
    public bool ShowPoweredBy { get; set; }      // Always true
    
    // Colours
    public string Primary { get; set; }
    public string PrimaryLight { get; set; }
    public string Background { get; set; }
    public string Surface { get; set; }
    public string TextPrimary { get; set; }
    public string TextSecondary { get; set; }
    public string Border { get; set; }
    public string Success { get; set; }
    public string Warning { get; set; }
    public string Error { get; set; }
    public string Info { get; set; }
    
    // Typography
    public string? FontFamily { get; set; }
    
    // Enterprise
    public string? CustomCss { get; set; }       // Enterprise only
    public PlanTier Plan { get; set; }
}
```

## Plan Gating

| Feature | Starter | Pro | Enterprise |
|---|---|---|---|
| Default WRKFLO branding | ✅ | ✅ | ✅ |
| Colour palette customisation | ❌ | ✅ | ✅ |
| Logo upload | ❌ | ✅ | ✅ |
| Custom font | ❌ | ❌ | ✅ |
| Custom CSS overrides | ❌ | ❌ | ✅ |
| "Powered by WRKFLO" badge | Always | Always | Always |

## Key Components

### ThemeProvider (`Components/Layout/ThemeProvider.razor`)

- Sits at the top of `MainLayout`
- Loads branding from `ITenantBrandingService`
- Renders CSS variables as a `<style>` block
- Generates a `MudTheme` to keep MudBlazor in sync
- Enterprise: injects custom CSS as a second `<style>` block
- Call `RefreshAsync()` to reload (e.g. after admin saves changes)

### AppLogo (`Components/Shared/AppLogo.razor`)

- Renders tenant logo or falls back to text
- Pro+ plans: displays uploaded logo image
- Starter: displays `TenantName` as text

### PoweredBy (`Components/Shared/PoweredBy.razor`)

- "Powered by WRKFLO" badge
- Always rendered — cannot be disabled
- Displayed in the footer

## Component Rules

When creating or editing components, follow these rules:

### DO ✅

```css
/* Use CSS variables */
.my-component {
    color: var(--wrkflo-primary);
    background: var(--wrkflo-surface);
    border: 1px solid var(--wrkflo-border);
    border-radius: var(--wrkflo-border-radius);
    font-family: var(--wrkflo-font-family);
}
```

```razor
@* Use MudBlazor Color enum where possible *@
<MudButton Color="Color.Primary">Click me</MudButton>
<MudIcon Color="Color.Success" />
```

### DON'T ❌

```css
/* Never hardcode hex colours */
.my-component {
    color: #4F46E5;        /* BAD — use var(--wrkflo-primary) */
    background: white;      /* BAD — use var(--wrkflo-surface) */
    border-color: #E5E7EB;  /* BAD — use var(--wrkflo-border) */
}
```

### Naming Convention

Use BEM-style class names for easy CSS targeting by Enterprise custom CSS:

```
.wrkflo-{component}__{element}--{modifier}

Examples:
.wrkflo-app-logo__img
.wrkflo-app-logo__text
.announcement-bar__content
.announcement-bar--info
.form-card-status--inbox
```

### Checklist for New Components

- [ ] No hardcoded hex colours — use `var(--wrkflo-*)` tokens
- [ ] No hardcoded font families — use `var(--wrkflo-font-family)`
- [ ] Use MudBlazor `Color.Primary` etc where possible
- [ ] Use BEM-style class names
- [ ] Keep HTML structure semantic (so custom CSS can target it)
- [ ] Add component-specific tokens if needed (e.g. `--wrkflo-card-padding`)

## Switching the Service Implementation

When ready for database-backed branding:

1. Create `DatabaseTenantBrandingService` implementing `ITenantBrandingService`
2. Query branding from DB based on authenticated user's tenant
3. Swap the DI registration in `Program.cs`:

```csharp
// Before
builder.Services.AddScoped<ITenantBrandingService, DefaultTenantBrandingService>();

// After
builder.Services.AddScoped<ITenantBrandingService, DatabaseTenantBrandingService>();
```

## Custom CSS (Enterprise)

Enterprise tenants can inject arbitrary CSS that loads after all other styles. This allows:

- Overriding specific component styles
- Adding custom animations
- Tweaking layout spacing
- Hiding/showing elements

The CSS is stored in `TenantBranding.CustomCss` and rendered by `ThemeProvider` inside a `<style id="wrkflo-custom-css">` block.

**Security note:** Custom CSS should be sanitised server-side to prevent CSS injection attacks (e.g. `url()` data exfiltration, `expression()` in legacy browsers).
