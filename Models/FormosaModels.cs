namespace Wrkflo.Shell.Models;

/// <summary>
/// Root form schema object. Every form is stored as one of these, serialised to JSON.
/// schemaVersion enables backwards-compatible migrations as the schema evolves.
/// </summary>
public class FormSchema
{
    public string SchemaVersion { get; set; } = "1.0";
    public string FormId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Untitled Form";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<FormSection> Sections { get; set; } = new();
}

/// <summary>
/// A horizontal band on the form. Contains 1–4 columns.
/// LayoutType drives the CSS grid template (e.g. "1-col", "2-col", "1-2-col").
/// </summary>
public class FormSection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = "Section";
    public string LayoutType { get; set; } = "1-col";
    public int Columns { get; set; } = 1;
    public List<FormRow> Rows { get; set; } = new();
}

/// <summary>
/// UI-only helper — not serialised to schema. Defines available layout options.
/// </summary>
public class FormLayoutDefinition
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public int ColumnCount { get; set; } = 1;
    public string GridTemplate { get; set; } = "1fr";
}

/// <summary>
/// A horizontal row of cells within a section.
/// </summary>
public class FormRow
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<FormCell> Cells { get; set; } = new();
}

/// <summary>
/// A single column cell within a row. Holds the elements dropped into it.
/// </summary>
public class FormCell
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int ColumnIndex { get; set; }
    public List<FormElement> Elements { get; set; } = new();
}

/// <summary>
/// A form element (label, input, etc.). Type drives which renderer is used.
/// Props is a flexible dictionary so each element type can carry its own data
/// without requiring schema changes for new element types.
/// </summary>
public class FormElement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string> Props { get; set; } = new();
}
