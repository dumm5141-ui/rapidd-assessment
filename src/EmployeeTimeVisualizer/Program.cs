using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeTimeVisualizer.Services;

namespace EmployeeTimeVisualizer;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine("===============================================================");
        Console.WriteLine("  RAPIDD ASSESSMENT - EMPLOYEE TIME VISUALIZER (C#)");
        Console.WriteLine("===============================================================");
        Console.WriteLine();

        try
        {
            // 1. Initialize modular services
            var apiService = new TimeEntryApiService();
            var reportService = new EmployeeReportService();
            var htmlGenerator = new HtmlReportGenerator();
            var chartGenerator = new PieChartGenerator();

            // 2. Fetch raw time entry data from Azure Functions endpoint
            Console.WriteLine("[1/4] Fetching time entries from Azure REST API...");
            var rawEntries = await apiService.FetchTimeEntriesAsync();
            Console.WriteLine($"      Successfully retrieved {rawEntries.Count} total records.");

            // 3. Process, clean, and summarize employee data
            Console.WriteLine("[2/4] Aggregating employee hours & applying data integrity rules...");
            var employees = reportService.ProcessEntries(rawEntries, unassignedFallbackName: "(Unassigned)");
            Console.WriteLine($"      Identified {employees.Count} unique active employees.");

            // Output files configuration
            string baseDirectory = Directory.GetCurrentDirectory();
            string htmlFilePath = Path.Combine(baseDirectory, "employees.html");
            string chartFilePath = Path.Combine(baseDirectory, "piechart.png");

            // 4. Generate the Pie Chart PNG image (Task b)
            Console.WriteLine("[3/4] Generating visual Pie Chart PNG...");
            chartGenerator.GeneratePieChart(employees, chartFilePath);
            Console.WriteLine($"      Saved chart image: {Path.GetFileName(chartFilePath)}");

            // 5. Generate the HTML Report table (Task a)
            Console.WriteLine("[4/4] Generating HTML Report table with threshold highlights...");
            string htmlContent = htmlGenerator.GenerateHtml(employees, pieChartRelativePath: "piechart.png");
            htmlGenerator.SaveToFile(htmlContent, htmlFilePath);
            Console.WriteLine($"      Saved HTML report: {Path.GetFileName(htmlFilePath)}");

            stopwatch.Stop();

            // Display Summary Table to Console
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine($" {"#",-3} | {"Employee Name",-24} | {"Total Hours",12} | {"Share (%)",10} | {"Status",-12}");
            Console.WriteLine("---------------------------------------------------------------------------------");

            int rank = 1;
            foreach (var emp in employees)
            {
                string status = emp.IsUnderThreshold ? "⚠ < 100 hrs" : "✓ Compliant";
                Console.WriteLine($" {rank,-3} | {emp.EmployeeName,-24} | {emp.TotalHoursWorked,9:F2} hrs | {emp.PercentageOfTotal,9:F2}% | {status,-12}");
                rank++;
            }

            Console.WriteLine("---------------------------------------------------------------------------------");
            double totalHours = employees.Sum(e => e.TotalHoursWorked);
            int lowCount = employees.Count(e => e.IsUnderThreshold);
            Console.WriteLine($" Total: {employees.Count} Employees | {totalHours:F2} Total Hours | {lowCount} Under 100 Hours");
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Output Files Generated:");
            Console.WriteLine($"  - HTML Table: {Path.GetFullPath(htmlFilePath)}");
            Console.WriteLine($"  - Pie Chart:  {Path.GetFullPath(chartFilePath)}");
            Console.WriteLine();
            Console.WriteLine($"Execution completed in {stopwatch.ElapsedMilliseconds} ms.");
            Console.WriteLine("===============================================================");

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"[ERROR] An unhandled exception occurred: {ex.Message}");
            Console.WriteLine(ex.ToString());
            Console.ResetColor();
            return 1;
        }
    }
}
