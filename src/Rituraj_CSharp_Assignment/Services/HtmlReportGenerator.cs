using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Generates an elegant, modern HTML report displaying employee time totals.
/// </summary>
public class HtmlReportGenerator : IHtmlReportGenerator
{
    public string GenerateHtml(IReadOnlyList<EmployeeSummary> employees, string? pieChartRelativePath = null)
    {
        var sb = new StringBuilder();
        double totalHours = employees.Sum(e => e.TotalHoursWorked);
        int lowHoursCount = employees.Count(e => e.IsUnderThreshold);
        double avgHours = employees.Count > 0 ? totalHours / employees.Count : 0;

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"UTF-8\">");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("  <title>Employee Time Tracking Report</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    :root {");
        sb.AppendLine("      --bg-primary: #f8fafc;");
        sb.AppendLine("      --card-bg: #ffffff;");
        sb.AppendLine("      --text-main: #0f172a;");
        sb.AppendLine("      --text-muted: #64748b;");
        sb.AppendLine("      --border: #e2e8f0;");
        sb.AppendLine("      --primary: #2563eb;");
        sb.AppendLine("      --danger-bg: #fee2e2;");
        sb.AppendLine("      --danger-border: #ef4444;");
        sb.AppendLine("      --danger-text: #991b1b;");
        sb.AppendLine("      --success-badge: #dcfce7;");
        sb.AppendLine("      --success-text: #166534;");
        sb.AppendLine("    }");
        sb.AppendLine("    * { box-sizing: border-box; margin: 0; padding: 0; }");
        sb.AppendLine("    body {");
        sb.AppendLine("      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;");
        sb.AppendLine("      background-color: var(--bg-primary);");
        sb.AppendLine("      color: var(--text-main);");
        sb.AppendLine("      padding: 40px 20px;");
        sb.AppendLine("      line-height: 1.5;");
        sb.AppendLine("    }");
        sb.AppendLine("    .container { max-width: 1100px; margin: 0 auto; }");
        sb.AppendLine("    .header { margin-bottom: 28px; }");
        sb.AppendLine("    .header h1 { font-size: 28px; font-weight: 700; color: var(--text-main); margin-bottom: 6px; }");
        sb.AppendLine("    .header p { color: var(--text-muted); font-size: 15px; }");
        sb.AppendLine("    .metrics-grid {");
        sb.AppendLine("      display: grid;");
        sb.AppendLine("      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));");
        sb.AppendLine("      gap: 16px;");
        sb.AppendLine("      margin-bottom: 30px;");
        sb.AppendLine("    }");
        sb.AppendLine("    .metric-card {");
        sb.AppendLine("      background: var(--card-bg);");
        sb.AppendLine("      border: 1px solid var(--border);");
        sb.AppendLine("      border-radius: 12px;");
        sb.AppendLine("      padding: 18px 20px;");
        sb.AppendLine("      box-shadow: 0 1px 3px rgba(0,0,0,0.05);");
        sb.AppendLine("    }");
        sb.AppendLine("    .metric-label { font-size: 13px; font-weight: 500; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }");
        sb.AppendLine("    .metric-value { font-size: 26px; font-weight: 700; color: var(--text-main); margin-top: 6px; }");
        sb.AppendLine("    .metric-card.alert .metric-value { color: var(--danger-border); }");
        sb.AppendLine("    .card {");
        sb.AppendLine("      background: var(--card-bg);");
        sb.AppendLine("      border: 1px solid var(--border);");
        sb.AppendLine("      border-radius: 14px;");
        sb.AppendLine("      box-shadow: 0 2px 6px rgba(0,0,0,0.04);");
        sb.AppendLine("      overflow: hidden;");
        sb.AppendLine("      margin-bottom: 36px;");
        sb.AppendLine("    }");
        sb.AppendLine("    .card-header { padding: 20px 24px; border-bottom: 1px solid var(--border); background-color: #fafbfc; }");
        sb.AppendLine("    .card-header h2 { font-size: 18px; font-weight: 600; color: var(--text-main); }");
        sb.AppendLine("    .card-header p { font-size: 13px; color: var(--text-muted); margin-top: 2px; }");
        sb.AppendLine("    table { width: 100%; border-collapse: collapse; text-align: left; }");
        sb.AppendLine("    th {");
        sb.AppendLine("      background-color: #f1f5f9;");
        sb.AppendLine("      color: #475569;");
        sb.AppendLine("      font-size: 12px;");
        sb.AppendLine("      font-weight: 600;");
        sb.AppendLine("      text-transform: uppercase;");
        sb.AppendLine("      letter-spacing: 0.6px;");
        sb.AppendLine("      padding: 14px 20px;");
        sb.AppendLine("      border-bottom: 1px solid var(--border);");
        sb.AppendLine("    }");
        sb.AppendLine("    td { padding: 14px 20px; font-size: 14px; border-bottom: 1px solid var(--border); }");
        sb.AppendLine("    tr:last-child td { border-bottom: none; }");
        sb.AppendLine("    /* Highlighting row for employees working less than 100 hours */");
        sb.AppendLine("    tr.low-hours {");
        sb.AppendLine("      background-color: var(--danger-bg) !important;");
        sb.AppendLine("      color: var(--danger-text);");
        sb.AppendLine("      font-weight: 500;");
        sb.AppendLine("    }");
        sb.AppendLine("    tr.low-hours td { border-bottom-color: #fca5a5; }");
        sb.AppendLine("    .rank { font-weight: 600; color: var(--text-muted); width: 60px; }");
        sb.AppendLine("    .emp-name { font-weight: 600; }");
        sb.AppendLine("    .hours-val { font-variant-numeric: tabular-nums; font-weight: 700; }");
        sb.AppendLine("    .badge {");
        sb.AppendLine("      display: inline-block;");
        sb.AppendLine("      padding: 4px 10px;");
        sb.AppendLine("      border-radius: 9999px;");
        sb.AppendLine("      font-size: 12px;");
        sb.AppendLine("      font-weight: 600;");
        sb.AppendLine("    }");
        sb.AppendLine("    .badge-ok { background-color: var(--success-badge); color: var(--success-text); }");
        sb.AppendLine("    .badge-alert { background-color: #fecaca; color: #991b1b; }");
        sb.AppendLine("    .chart-container {");
        sb.AppendLine("      padding: 24px;");
        sb.AppendLine("      text-align: center;");
        sb.AppendLine("      background: var(--card-bg);");
        sb.AppendLine("    }");
        sb.AppendLine("    .chart-container img {");
        sb.AppendLine("      max-width: 100%;");
        sb.AppendLine("      height: auto;");
        sb.AppendLine("      border-radius: 12px;");
        sb.AppendLine("      box-shadow: 0 4px 12px rgba(0,0,0,0.06);");
        sb.AppendLine("    }");
        sb.AppendLine("    .footer {");
        sb.AppendLine("      text-align: center;");
        sb.AppendLine("      font-size: 13px;");
        sb.AppendLine("      color: var(--text-muted);");
        sb.AppendLine("      margin-top: 40px;");
        sb.AppendLine("    }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <div class=\"container\">");
        sb.AppendLine("    <header class=\"header\">");
        sb.AppendLine("      <h1>Employee Working Time Report</h1>");
        sb.AppendLine("      <p>Automated summary ordered by total time worked. Highlighted rows indicate employees below the 100-hour threshold.</p>");
        sb.AppendLine("    </header>");

        // Top Metrics Grid
        sb.AppendLine("    <div class=\"metrics-grid\">");
        sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-label\">Total Work Hours</div><div class=\"metric-value\">{totalHours:F2} hrs</div></div>");
        sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-label\">Total Employees</div><div class=\"metric-value\">{employees.Count}</div></div>");
        sb.AppendLine($"      <div class=\"metric-card\"><div class=\"metric-label\">Average / Employee</div><div class=\"metric-value\">{avgHours:F2} hrs</div></div>");
        sb.AppendLine($"      <div class=\"metric-card alert\"><div class=\"metric-label\">Under 100 Hours</div><div class=\"metric-value\">{lowHoursCount} {(lowHoursCount == 1 ? "Employee" : "Employees")}</div></div>");
        sb.AppendLine("    </div>");

        // Table Card
        sb.AppendLine("    <div class=\"card\">");
        sb.AppendLine("      <div class=\"card-header\">");
        sb.AppendLine("        <h2>Employee Work Hours Breakdown</h2>");
        sb.AppendLine("        <p>Sorted by Total Time Worked in descending order</p>");
        sb.AppendLine("      </div>");
        sb.AppendLine("      <table>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("          <tr>");
        sb.AppendLine("            <th class=\"rank\">Rank</th>");
        sb.AppendLine("            <th>Name</th>");
        sb.AppendLine("            <th>Total Time Worked</th>");
        sb.AppendLine("            <th>Work Share</th>");
        sb.AppendLine("            <th>Status</th>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");

        int rank = 1;
        foreach (var emp in employees)
        {
            string rowClass = emp.IsUnderThreshold ? " class=\"low-hours\"" : "";
            string statusBadge = emp.IsUnderThreshold
                ? "<span class=\"badge badge-alert\">&lt; 100 Hours</span>"
                : "<span class=\"badge badge-ok\">Compliant</span>";

            sb.AppendLine($"          <tr{rowClass}>");
            sb.AppendLine($"            <td class=\"rank\">#{rank}</td>");
            sb.AppendLine($"            <td class=\"emp-name\">{System.Net.WebUtility.HtmlEncode(emp.EmployeeName)}</td>");
            sb.AppendLine($"            <td class=\"hours-val\">{emp.TotalHoursWorked:F2} hrs</td>");
            sb.AppendLine($"            <td>{emp.PercentageOfTotal:F2}%</td>");
            sb.AppendLine($"            <td>{statusBadge}</td>");
            sb.AppendLine("          </tr>");
            rank++;
        }

        sb.AppendLine("        </tbody>");
        sb.AppendLine("      </table>");
        sb.AppendLine("    </div>");

        // Optional embedded pie chart
        if (!string.IsNullOrWhiteSpace(pieChartRelativePath))
        {
            sb.AppendLine("    <div class=\"card\">");
            sb.AppendLine("      <div class=\"card-header\">");
            sb.AppendLine("        <h2>Visual Workload Distribution (Pie Chart)</h2>");
            sb.AppendLine("        <p>Proportion of total company time contributed by each team member</p>");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <div class=\"chart-container\">");
            sb.AppendLine($"        <img src=\"{System.Net.WebUtility.HtmlEncode(pieChartRelativePath)}\" alt=\"Employee Total Time Worked Pie Chart\" />");
            sb.AppendLine("      </div>");
            sb.AppendLine("    </div>");
        }

        sb.AppendLine("    <footer class=\"footer\">");
        sb.AppendLine($"      <p>Report generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC &bull; Rituraj C# Assessment</p>");
        sb.AppendLine("    </footer>");
        sb.AppendLine("  </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
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
