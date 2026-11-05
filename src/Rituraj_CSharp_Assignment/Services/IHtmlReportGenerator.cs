using System.Collections.Generic;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Service interface for generating an HTML report with employee work hours.
/// </summary>
public interface IHtmlReportGenerator
{
    /// <summary>
    /// Generates HTML markup containing a styled table and optional chart visualization.
    /// </summary>
    string GenerateHtml(IReadOnlyList<EmployeeSummary> employees, string? pieChartRelativePath = null);

    /// <summary>
    /// Saves the generated HTML string to the designated output file path.
    /// </summary>
    void SaveToFile(string html, string filePath);
}
