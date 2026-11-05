using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeVisualizer.Models;

namespace EmployeeTimeVisualizer.Services;

/// <summary>
/// Aggregates and transforms raw time entries into ordered employee summaries.
/// </summary>
public class EmployeeReportService
{
    public const string DefaultUnassignedLabel = "(Unassigned)";

    /// <summary>
    /// Processes raw time entries into ordered employee summaries.
    /// Preserves original employee names from the API. Genuinely anonymous or missing
    /// records are grouped under a neutral "(Unassigned)" label.
    /// </summary>
    public IReadOnlyList<EmployeeSummary> ProcessEntries(
        IEnumerable<TimeEntry> rawEntries,
        string unassignedFallbackName = DefaultUnassignedLabel)
    {
        if (rawEntries == null)
        {
            return Array.Empty<EmployeeSummary>();
        }

        // 1. Exclude deleted records and records without valid durations
        var activeEntries = rawEntries
            .Where(e => e.DeletedOn == null)
            .Select(e => new
            {
                // Preserve genuine API names; assign neutral label only when name is null or whitespace
                Name = string.IsNullOrWhiteSpace(e.EmployeeName)
                    ? unassignedFallbackName
                    : e.EmployeeName.Trim(),
                Duration = e.GetDurationHours()
            })
            .Where(e => e.Duration > 0)
            .ToList();

        if (activeEntries.Count == 0)
        {
            return Array.Empty<EmployeeSummary>();
        }

        // 2. Group by employee name and aggregate hours and entry counts
        var grouped = activeEntries
            .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => new
            {
                Name = g.Key,
                TotalHours = g.Sum(x => x.Duration),
                Count = g.Count()
            })
            .OrderByDescending(x => x.TotalHours)
            .ToList();

        // 3. Compute overall team total hours to derive percentages
        double overallHours = grouped.Sum(x => x.TotalHours);

        var summaries = grouped.Select(g => new EmployeeSummary
        {
            EmployeeName = g.Name,
            TotalHoursWorked = Math.Round(g.TotalHours, 2),
            EntryCount = g.Count,
            PercentageOfTotal = overallHours > 0
                ? Math.Round((g.TotalHours / overallHours) * 100.0, 2)
                : 0.0
        }).ToList();

        return summaries;
    }
}
