using System.Collections.Generic;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Service interface for rendering employee work percentages as a PNG pie chart.
/// </summary>
public interface IPieChartGenerator
{
    /// <summary>
    /// Generates a PNG pie chart image and saves it to the specified file path.
    /// </summary>
    /// <param name="employees">Ordered list of employee summaries.</param>
    /// <param name="outputFilePath">Destination path for the PNG file.</param>
    void GeneratePieChart(IReadOnlyList<EmployeeSummary> employees, string outputFilePath);
}
