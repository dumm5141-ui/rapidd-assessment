using System.Collections.Generic;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Service interface for aggregating, cleaning, and sorting employee time records.
/// </summary>
public interface IEmployeeReportService
{
    /// <summary>
    /// Processes raw time entries into ordered employee summaries.
    /// </summary>
    /// <param name="rawEntries">Collection of raw time entries.</param>
    /// <param name="unassignedFallbackName">Realistic fallback name to satisfy data integrity rules when name is missing.</param>
    /// <returns>List of summaries sorted descending by total hours worked.</returns>
    IReadOnlyList<EmployeeSummary> ProcessEntries(IEnumerable<TimeEntry> rawEntries, string unassignedFallbackName = "Vijay (Unassigned)");
}
