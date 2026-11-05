using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using EmployeeTimeVisualizer.Models;

namespace EmployeeTimeVisualizer.Services;

/// <summary>
/// Generates a clean, simple HTML report displaying employee time totals.
/// </summary>
public class HtmlReportGenerator
{
    public string GenerateHtml(IReadOnlyList<EmployeeSummary> employees, string? pieChartRelativePath = null)
    {
        var rows = string.Join(Environment.NewLine, employees.Select((emp, index) =>
            $$"""
                  <tr{{(emp.IsUnderThreshold ? " class=\"low-hours\"" : "")}}>
                    <td>{{index + 1}}</td>
                    <td>{{WebUtility.HtmlEncode(emp.EmployeeName)}}</td>
                    <td>{{emp.TotalHoursWorked:F2}} hrs</td>
                    <td>{{emp.PercentageOfTotal:F2}}%</td>
                  </tr>
            """));

        var chartHtml = string.IsNullOrWhiteSpace(pieChartRelativePath) ? "" :
            $$"""
              <div class="chart-container">
                <h2>Time Distribution</h2>
                <img src="{{WebUtility.HtmlEncode(pieChartRelativePath)}}" alt="Employee Pie Chart" />
              </div>
            """;

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>Employee Time Report</title>
          <style>
            body { font-family: Arial, sans-serif; max-width: 900px; margin: 30px auto; padding: 0 16px; color: #333; line-height: 1.4; }
            h1 { margin-bottom: 6px; }
            p { color: #666; margin-bottom: 20px; }
            table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }
            th, td { border: 1px solid #ddd; padding: 10px 14px; text-align: left; }
            th { background-color: #f2f2f2; font-weight: bold; }
            tr.low-hours { background-color: #fee2e2; color: #991b1b; }
            .chart-container { text-align: center; margin-top: 30px; }
            .chart-container img { max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 6px; }
          </style>
        </head>
        <body>
          <h1>Employee Working Time Report</h1>
          <p>Employees ordered by total time worked. Rows highlighted in red indicate less than 100 hours worked.</p>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Total Time Worked</th>
                <th>Percentage</th>
              </tr>
            </thead>
            <tbody>
        {{rows}}
            </tbody>
          </table>
        {{chartHtml}}
        </body>
        </html>

        """;
    }

    public void SaveToFile(string html, string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, html, Encoding.UTF8);
    }
}
