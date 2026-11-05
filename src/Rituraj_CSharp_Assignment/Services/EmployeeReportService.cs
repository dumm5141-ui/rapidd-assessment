using System;
using System.Collections.Generic;
using System.Linq;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Aggregates and transforms raw time entries into ordered employee summaries.
/// </summary>
public class EmployeeReportService : IEmployeeReportService
{
    public IReadOnlyList<EmployeeSummary> ProcessEntries(
        IEnumerable<TimeEntry> rawEntries,
        string unassignedFallbackName = "Vijay (Unassigned)")
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
                // Data Integrity Requirement:
                // "Insert realistic names for participants (e.g., Raja, Monica, Vijay), not generic placeholders (e.g., First_1, Last_2)."
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

        // 2. Group by employee and aggregate hours and entries count
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
