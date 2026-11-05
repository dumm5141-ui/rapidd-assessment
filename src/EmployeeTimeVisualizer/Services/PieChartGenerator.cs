using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EmployeeTimeVisualizer.Models;
using ScottPlot;

namespace EmployeeTimeVisualizer.Services;

/// <summary>
/// Generates a PNG Pie Chart displaying employee working time distribution using ScottPlot 5.
/// </summary>
public class PieChartGenerator
{
    public void GeneratePieChart(IReadOnlyList<EmployeeSummary> employees, string outputFilePath)
    {
        ConfigureFont();

        EnsureDirectoryExists(outputFilePath);

        if (employees == null || employees.Count == 0)
        {
            var emptyPlot = new Plot();
            emptyPlot.FigureBackground.Color = Color.FromHex("#f8fafc");
            emptyPlot.HideGrid();
            emptyPlot.Axes.Frameless();
            var anno = emptyPlot.Add.Annotation("No data available to display.", Alignment.MiddleCenter);
            if (!string.IsNullOrEmpty(Fonts.Default)) anno.LabelFontName = Fonts.Default;
            anno.LabelFontSize = 18;
            anno.LabelFontColor = Color.FromHex("#64748b");
            anno.LabelBackgroundColor = Colors.Transparent;
            anno.LabelBorderColor = Colors.Transparent;
            anno.LabelShadowColor = Colors.Transparent;
            emptyPlot.SavePng(outputFilePath, 800, 600);
            return;
        }

        double totalHours = employees.Sum(e => e.TotalHoursWorked);
        if (totalHours <= 0)
        {
            var zeroPlot = new Plot();
            zeroPlot.FigureBackground.Color = Color.FromHex("#f8fafc");
            zeroPlot.HideGrid();
            zeroPlot.Axes.Frameless();
            var anno = zeroPlot.Add.Annotation("Total worked hours is zero.", Alignment.MiddleCenter);
            if (!string.IsNullOrEmpty(Fonts.Default)) anno.LabelFontName = Fonts.Default;
            anno.LabelFontSize = 18;
            anno.LabelFontColor = Color.FromHex("#64748b");
            anno.LabelBackgroundColor = Colors.Transparent;
            anno.LabelBorderColor = Colors.Transparent;
            anno.LabelShadowColor = Colors.Transparent;
            zeroPlot.SavePng(outputFilePath, 800, 600);
            return;
        }

        var plot = new Plot();
        plot.FigureBackground.Color = Color.FromHex("#f8fafc");

        var pie = plot.Add.Pie(employees.Select(e => e.TotalHoursWorked).ToArray());
        pie.DonutFraction = 0.5;

        for (int i = 0; i < employees.Count && i < pie.Slices.Count; i++)
        {
            var emp = employees[i];
            pie.Slices[i].Label = $"{emp.PercentageOfTotal:F1}%";
            pie.Slices[i].LegendText = $"{emp.EmployeeName}  •  {emp.TotalHoursWorked:F2} hrs  ({emp.PercentageOfTotal:F2}%)";
        }

        plot.HideGrid();
        plot.Axes.Frameless();

        plot.ShowLegend(Edge.Right);
        plot.Legend.FontSize = 13;
        plot.Legend.BackgroundColor = Colors.White;
        plot.Legend.OutlineColor = Color.FromHex("#e2e8f0");

        plot.SavePng(outputFilePath, 1050, 700);
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static void ConfigureFont()
    {
        if (!string.IsNullOrEmpty(Fonts.Default)) return;

        try
        {
            var psi = new ProcessStartInfo("fc-match", "-f %{file} sans-serif")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            string file = proc?.StandardOutput.ReadToEnd().Trim() ?? "";
            if (File.Exists(file))
            {
                Fonts.AddFontFile("AppFont", file);
                Fonts.Default = "AppFont";
                return;
            }
        }
        catch { }

        string[] candidateDirs = {
            "/usr/share/fonts",
            "/usr/local/share/fonts",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/share/fonts"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fonts"),
            "/run/current-system/sw/share/fonts",
            "/etc/profiles/per-user",
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts)
        };

        foreach (var dir in candidateDirs)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    var font = Directory.EnumerateFiles(dir, "*.ttf", SearchOption.AllDirectories)
                        .FirstOrDefault(f => f.Contains("Sans", StringComparison.OrdinalIgnoreCase) ||
                                             f.Contains("DejaVu", StringComparison.OrdinalIgnoreCase) ||
                                             f.Contains("Arial", StringComparison.OrdinalIgnoreCase) ||
                                             f.Contains("Roboto", StringComparison.OrdinalIgnoreCase) ||
                                             f.Contains("Ubuntu", StringComparison.OrdinalIgnoreCase));
                    if (font != null)
                    {
                        Fonts.AddFontFile("AppFont", font);
                        Fonts.Default = "AppFont";
                        return;
                    }
                }
                catch { }
            }
        }
    }
}
