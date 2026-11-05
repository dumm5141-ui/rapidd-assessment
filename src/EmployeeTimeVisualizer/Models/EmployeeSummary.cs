namespace EmployeeTimeVisualizer.Models;

/// <summary>
/// Represents the aggregated working time summary for an individual employee.
/// </summary>
public class EmployeeSummary
{
    /// <summary>
    /// Employee name.
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Total hours worked across all valid entries.
    /// </summary>
    public double TotalHoursWorked { get; set; }

    /// <summary>
    /// Total number of valid entries recorded for this employee.
    /// </summary>
    public int EntryCount { get; set; }

    /// <summary>
    /// Percentage of the entire team's total worked hours.
    /// </summary>
    public double PercentageOfTotal { get; set; }

    /// <summary>
    /// Indicates whether the employee worked less than the required 100 hours threshold.
    /// </summary>
    public bool IsUnderThreshold => TotalHoursWorked < 100.0;
}
