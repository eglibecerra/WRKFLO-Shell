namespace Wrkflo.Shell.Models;

/// <summary>
/// Status of a workflow step.
/// </summary>
public enum StepStatus
{
    Completed,
    Active,
    Pending,
    Skipped
}

/// <summary>
/// Represents a single step in a branching workflow.
/// </summary>
public class BranchingStep
{
    /// <summary>Step display name (e.g. "Manager Approval").</summary>
    public string Name { get; set; } = "";

    /// <summary>Role or team responsible (e.g. "Direct Manager").</summary>
    public string Role { get; set; } = "";

    /// <summary>Current status of this step.</summary>
    public StepStatus Status { get; set; } = StepStatus.Pending;

    /// <summary>Who actioned this step (shown in tooltip).</summary>
    public string? ActionedBy { get; set; }

    /// <summary>When the step was actioned (shown in tooltip).</summary>
    public DateTime? ActionedDate { get; set; }

    /// <summary>Optional comment left by the actioner.</summary>
    public string? Comment { get; set; }

    /// <summary>Reason this step was skipped (when Status = Skipped).</summary>
    public string? SkipReason { get; set; }

    /// <summary>Label describing the branch condition (e.g. "Amount threshold").</summary>
    public string? BranchConditionLabel { get; set; }

    /// <summary>Possible branch paths from this step. Null if no branching.</summary>
    public List<BranchPath>? Branches { get; set; }
}

/// <summary>
/// Represents one possible path at a branch point.
/// </summary>
public class BranchPath
{
    /// <summary>Condition that triggers this path (e.g. "Amount > $5,000").</summary>
    public string Condition { get; set; } = "";

    /// <summary>Name of the step this path leads to.</summary>
    public string TargetStepName { get; set; } = "";

    /// <summary>Whether this path was the one actually taken.</summary>
    public bool WasTaken { get; set; }
}
