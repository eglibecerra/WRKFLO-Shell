# Blazor Learning Path — Eggs' Guide
> From zero to production-ready. All examples use WRKFLO-Shell as the real codebase.

---

## 🟢 Stage 1: Foundations

### 1.1 What is Blazor?
- Blazor is a .NET web framework — write C# instead of JavaScript
- Two hosting models:
  - **Blazor Server** — C# runs on the server, UI updates via SignalR (WebSocket)
  - **Blazor WebAssembly (WASM)** — C# runs in the browser via WebAssembly
  - **Blazor United / Auto** (.NET 8+) — SSR + interactive islands, best of both
- WRKFLO uses **Blazor Server** (real-time, no WASM download, great for internal tools)

### 1.2 Project Structure
```
Program.cs          → App entry point, DI registrations, middleware
App.razor           → Root component, error boundary
Routes.razor        → Router, default layout
Components/
  Layout/           → Layouts (MainLayout, AuthLayout)
  Pages/            → Routable pages (@page directive)
  Shared/           → Reusable components (no @page)
wwwroot/            → Static files (CSS, JS, images)
```

### 1.3 The @page Directive
```razor
@page "/login"           ← makes this component a routable page
@page "/signin"          ← can have multiple routes on one component
```

### 1.4 Layouts
- `@inherits LayoutComponentBase` → marks a component as a layout
- `@Body` → where child page content renders
- Set default in `Routes.razor`: `DefaultLayout="typeof(MainLayout)"`
- Override per page: `@layout AuthLayout`
- Remove layout: `@layout null`

### 1.5 Components
- Every `.razor` file is a component
- Components = HTML markup + optional `@code { }` block
- Without `@page` = reusable component (like `<MyButton />`)
- With `@page` = routable page

---

## 🟡 Stage 2: Component Basics

### 2.1 Data Binding
```razor
@* One-way (display only) *@
<p>Hello @name</p>

@* Two-way binding (input ↔ field) *@
<input @bind="name" />

@* Two-way with event control *@
<input @bind="name" @bind:event="oninput" />

@code {
    private string name = "Eggs";
}
```

### 2.2 Parameters (Props)
```razor
@* Child component *@
<MudButton Color="@Color">@Label</MudButton>

@code {
    [Parameter] public string Label { get; set; } = "Click me";
    [Parameter] public Color Color { get; set; } = Color.Primary;
}

@* Parent passes in *@
<MyButton Label="Save" Color="Color.Success" />
```

### 2.3 Event Callbacks
```razor
@* Child fires an event *@
<MudButton OnClick="OnSaveClicked">Save</MudButton>

@code {
    [Parameter] public EventCallback OnSave { get; set; }

    private async Task OnSaveClicked() => await OnSave.InvokeAsync();
}

@* Parent listens *@
<MyButton OnSave="HandleSave" />

@code {
    private void HandleSave() => Console.WriteLine("Saved!");
}
```

### 2.4 Lifecycle Methods
```csharp
// Runs once on first load (use for data fetching)
protected override async Task OnInitializedAsync()
{
    _forms = await FormsService.GetAllAsync();
}

// Runs when [Parameter] values change
protected override void OnParametersSet() { }

// Runs after render — firstRender=true only once
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // JS interop goes here
    }
}

// Cleanup — implement IDisposable
public void Dispose()
{
    // Unsubscribe from events, cancel timers etc.
}
```

### 2.5 Conditional Rendering
```razor
@if (_isLoading)
{
    <MudProgressLinear Indeterminate="true" />
}
else if (_forms.Count == 0)
{
    <MudText>No forms yet.</MudText>
}
else
{
    <FormList Forms="_forms" />
}
```

### 2.6 Loops
```razor
@foreach (var form in _forms)
{
    <FormCard Form="form" Key="@form.Id" />
}
```

---

## 🟠 Stage 3: Intermediate

### 3.1 Dependency Injection
```razor
@* In a component *@
@inject IFormsService FormsService
@inject NavigationManager Nav
@inject IStringLocalizer<MyPage> L

@* In code-behind or service *@
public class MyService(IDbContext db, ILogger<MyService> logger)
{
    // Constructor injection
}
```

### 3.2 Services Pattern
```csharp
// Define interface
public interface IFormsService
{
    Task<List<Form>> GetAllAsync();
    Task<Form> GetByIdAsync(Guid id);
    Task CreateAsync(Form form);
}

// Register in Program.cs
builder.Services.AddScoped<IFormsService, FormsService>();

// Use in component
@inject IFormsService FormsService
```

### 3.3 Forms & Validation
```razor
<EditForm Model="_model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <MudTextField @bind-Value="_model.Title"
                  For="@(() => _model.Title)" />

    <MudButton ButtonType="ButtonType.Submit">Submit</MudButton>
</EditForm>

@code {
    private FormModel _model = new();

    private async Task HandleSubmit()
    {
        // Only called if model is valid
        await FormsService.CreateAsync(_model);
    }
}
```

### 3.4 State Management
- **Component state** — fields in `@code` block (lost on navigate away)
- **Scoped service** — injected service with state, lives for the SignalR circuit (Blazor Server)
- **Cascading values** — pass data down the component tree without prop drilling
- **Fluxor / other state libs** — Redux-style for complex apps

```razor
@* Cascading value — parent sets it *@
<CascadingValue Value="_currentUser">
    @Body
</CascadingValue>

@* Deeply nested child gets it *@
[CascadingParameter] private UserContext CurrentUser { get; set; } = default!;
```

### 3.5 JS Interop
```razor
@inject IJSRuntime JS

@code {
    private async Task FocusInput()
    {
        await JS.InvokeVoidAsync("focusElement", "#myInput");
    }

    private async Task<string> GetClipboard()
    {
        return await JS.InvokeAsync<string>("navigator.clipboard.readText");
    }
}
```

### 3.6 Localisation (i18n)
```razor
@inject IStringLocalizer<MyPage> L

<MudText>@L["WelcomeBack"]</MudText>
<MudText>@string.Format(L["SignInSubtitle"], tenantName)</MudText>
```
- Resx files in `Resources/` mirror component folder structure
- Default culture set in `Program.cs`

---

## 🔴 Stage 4: Advanced

### 4.1 Authentication & Authorization
```razor
@* Protect a page *@
@attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]

@* Conditional UI *@
<AuthorizeView>
    <Authorized>
        <p>Welcome @context.User.Identity?.Name</p>
    </Authorized>
    <NotAuthorized>
        <MudButton Href="/login">Sign In</MudButton>
    </NotAuthorized>
</AuthorizeView>
```

### 4.2 Custom AuthenticationStateProvider
```csharp
public class WrkfloAuthStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        var identity = token is null
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaims(token), "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
```

### 4.3 SignalR & Real-time Updates
```csharp
// Blazor Server already runs over SignalR
// For custom real-time (e.g. live approval status):
protected override async Task OnInitializedAsync()
{
    _hubConnection = new HubConnectionBuilder()
        .WithUrl("/hubs/approvals")
        .Build();

    _hubConnection.On<Guid>("ApprovalUpdated", async (id) =>
    {
        await RefreshApproval(id);
        await InvokeAsync(StateHasChanged); // ← crucial on background threads
    });

    await _hubConnection.StartAsync();
}
```

### 4.4 RenderFragment (Slots)
```razor
@* Component accepts content as a slot *@
<WrkfloCard>
    <HeaderContent>
        <MudText Typo="Typo.h6">My Title</MudText>
    </HeaderContent>
    <ChildContent>
        <p>The card body goes here.</p>
    </ChildContent>
</WrkfloCard>

@* Component definition *@
@code {
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

### 4.5 Performance
- `@key` directive — helps Blazor diff lists efficiently
- `ShouldRender()` — skip re-render if nothing changed
- `StateHasChanged()` — manually trigger re-render (needed in async callbacks)
- Virtualization for long lists:
```razor
<Virtualize Items="_forms" Context="form">
    <FormCard Form="form" />
</Virtualize>
```

### 4.6 Multi-tenancy Patterns
- Resolve tenant from subdomain/header in middleware
- Scope DI services per-tenant (custom `IServiceProvider` factory)
- CSS variables for per-tenant theming (already in WRKFLO!)
- DB schemas or row-level security per tenant

### 4.7 Testing Blazor Components
```csharp
// bUnit — the Blazor component testing library
[Fact]
public void LoginForm_ShowsError_WhenEmailInvalid()
{
    using var ctx = new TestContext();
    var cut = ctx.RenderComponent<Login>();

    cut.Find("input[type=email]").Change("notanemail");
    cut.Find("form").Submit();

    cut.Find(".validation-message").MarkupMatches("<div>Enter a valid email</div>");
}
```

---

## 📋 What to Build (in order)
1. ✅ Login page (done!)
2. Auth wiring (Microsoft.Identity.Web + AuthStateProvider)
3. Protected routes (AuthorizeRouteView)
4. Dashboard home (stats, recent forms, quick actions)
5. Form Builder (Formosa — the big one)
6. Workflow designer (approval steps)
7. Submissions list + detail view
8. Admin panel (users, branding, settings)
9. Real-time notifications (SignalR hub)
10. Agent API (OpenAPI-first, MCP integration)

---

*Updated: 2026-02-22 | Questions → just ask Pepe 🐸*
