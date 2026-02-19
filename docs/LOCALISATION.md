# Localisation Guide

WRKFLO uses .NET's built-in `IStringLocalizer<T>` for globalisation. All user-facing strings are stored in `.resx` resource files and injected into components.

## How It Works

1. Each component has a matching `.resx` file in `Resources/` that mirrors the component path
2. Components inject `IStringLocalizer<ComponentName>` and reference strings by key
3. The framework automatically picks the correct `.resx` based on the user's culture

## Folder Structure

```
Resources/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.resx              ← default (en-AU)
│   │   ├── MainLayout.es.resx           ← Spanish
│   │   ├── MainLayout.fr.resx           ← French
│   │   └── ReconnectModal.resx
│   └── Shared/
│       ├── AnnouncementBar.resx
│       ├── FormCard.resx
│       ├── NavSearch.resx
│       ├── StatusTabs.resx
│       └── WorkflowStepsView.resx
└── Pages/
    └── (future page-level resources)
```

## Adding a New Language

### Step 1: Pick the culture code

Use standard .NET culture codes: `es` (Spanish), `fr` (French), `de` (German), `pt` (Portuguese), `ja` (Japanese), `zh` (Chinese), etc.

For regional variants use: `es-MX` (Mexican Spanish), `pt-BR` (Brazilian Portuguese), `en-GB` (British English), etc.

### Step 2: Register the culture in Program.cs

Open `Program.cs` and add the new `CultureInfo` to the `supportedCultures` array:

```csharp
var supportedCultures = new[]
{
    new CultureInfo("en-AU"),   // Default
    new CultureInfo("en"),
    new CultureInfo("es"),      // Spanish
    new CultureInfo("fr"),      // French
    new CultureInfo("de"),      // German
    new CultureInfo("pt"),      // Portuguese
    new CultureInfo("ja"),      // Japanese
    new CultureInfo("zh"),      // Chinese
    new CultureInfo("ko"),      // ← ADD NEW CULTURE HERE
};
```

### Step 3: Create .resx files for each component

For every `.resx` file in `Resources/`, create a copy with the culture suffix.

Example — adding Spanish (`es`):

| Default (English) | Spanish |
|---|---|
| `AnnouncementBar.resx` | `AnnouncementBar.es.resx` |
| `NavSearch.resx` | `NavSearch.es.resx` |
| `StatusTabs.resx` | `StatusTabs.es.resx` |
| `FormCard.resx` | `FormCard.es.resx` |
| `WorkflowStepsView.resx` | `WorkflowStepsView.es.resx` |
| `MainLayout.resx` | `MainLayout.es.resx` |
| `ReconnectModal.resx` | `ReconnectModal.es.resx` |

### Step 4: Translate the values

Open each new `.es.resx` file and translate the `<value>` elements. Keep the `name` keys identical.

Example — `StatusTabs.es.resx`:
```xml
<data name="Inbox" xml:space="preserve">
    <value>Bandeja de entrada</value>
</data>
<data name="Outbox" xml:space="preserve">
    <value>Bandeja de salida</value>
</data>
<data name="Complete" xml:space="preserve">
    <value>Completado</value>
</data>
```

### Step 5: Build and test

```bash
dotnet build
dotnet run
```

The framework falls back gracefully:
- Request for `es-MX` → tries `es-MX.resx` → falls back to `es.resx` → falls back to default `.resx`

## Component Reference

All localisable strings by component:

### AnnouncementBar
| Key | Default (en) |
|---|---|
| `Dismiss` | Dismiss |

### NavSearch
| Key | Default (en) |
|---|---|
| `Placeholder` | Search forms, workflows... |
| `NoResultsFound` | No results found |

### StatusTabs
| Key | Default (en) |
|---|---|
| `Inbox` | Inbox |
| `Outbox` | Outbox |
| `Complete` | Complete |

### FormCard
| Key | Default (en) |
|---|---|
| `Redirect` | REDIRECT |
| `View` | VIEW |

### WorkflowStepsView
| Key | Default (en) |
|---|---|
| `FullMap` | Full Map |
| `LinearView` | Linear View |
| `SwitchToLinearView` | Switch to linear view |
| `ShowFullWorkflowMap` | Show full workflow map |
| `ActionedOn` | Actioned on |
| `By` | By |
| `ForwardedTo` | Forwarded to |
| `On` | on |
| `SkippedConditionNotMet` | Skipped — condition not met |
| `Pending` | Pending |
| `BranchPoint` | Branch Point |
| `BranchPointPossiblePaths` | Branch point: {0} possible paths |
| `Taken` | Taken |

> Note: `{0}` in `BranchPointPossiblePaths` is a format placeholder — use `string.Format()` in the component.

### MainLayout
| Key | Default (en) |
|---|---|
| `AppTitle` | WrkFlo |
| `Dashboard` | Dashboard |
| `Forms` | Forms |
| `Workflows` | Workflows |
| `Admin` | Admin |
| `Users` | Users |
| `Settings` | Settings |
| `Branding` | Branding |
| `General` | General |
| `Notifications` | Notifications |
| `Security` | Security |
| `ComponentShowcase` | Component Showcase |
| `CustomComponents` | Custom Components |
| `Messages` | Messages |
| `MyProfile` | My Profile |
| `SignOut` | Sign Out |
| `SearchForms` | Search Forms |
| `FooterText` | WrkFlo © 2026 by { bectec } |

### ReconnectModal
| Key | Default (en) |
|---|---|
| `Rejoining` | Rejoining the server... |
| `RejoinFailed` | Rejoin failed... trying again in |
| `Seconds` | seconds. |
| `FailedToRejoin` | Failed to rejoin. Please retry or reload the page. |
| `Retry` | Retry |
| `SessionPaused` | The session has been paused by the server. |
| `FailedToResume` | Failed to resume the session. Please retry or reload the page. |
| `Resume` | Resume |

## Adding Strings to a New Component

When creating a new custom component:

1. Create `Resources/Components/Shared/YourComponent.resx` with English defaults
2. Add `@inject IStringLocalizer<YourComponent> L` at the top of the `.razor` file
3. Use `@L["KeyName"]` instead of hardcoded strings
4. For format strings use `@string.Format(L["KeyName"], value1, value2)`
5. Add translations to `YourComponent.{culture}.resx` for each supported language

## Culture Switching (Future)

Currently the culture is set server-side via `RequestLocalizationOptions`. Future options:
- User preference stored in database (per-tenant or per-user)
- Culture picker dropdown in the user menu
- Browser `Accept-Language` header detection (already supported by the middleware)
