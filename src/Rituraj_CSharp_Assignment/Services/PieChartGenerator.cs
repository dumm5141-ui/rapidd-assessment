using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rituraj_CSharp_Assignment.Models;
using SkiaSharp;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Cross-platform PNG Pie Chart generator using SkiaSharp.
/// Produces an elegant, high-resolution visual breakdown of employee working percentages.
/// </summary>
public class PieChartGenerator : IPieChartGenerator
{
    private static readonly string[] PaletteHex = new[]
    {
        "#2563eb", // Blue
        "#0d9488", // Teal
        "#10b981", // Emerald
        "#84cc16", // Lime
        "#eab308", // Amber
        "#f97316", // Orange
        "#ec4899", // Pink
        "#8b5cf6", // Purple
        "#06b6d4", // Cyan
        "#6366f1", // Indigo
        "#ef4444", // Rose / Red
        "#64748b", // Slate
        "#a855f7", // Violet
        "#14b8a6"  // Light Teal
    };

    public void GeneratePieChart(IReadOnlyList<EmployeeSummary> employees, string outputFilePath)
    {
        int width = 1050;
        int height = 700;

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColor.Parse("#f8fafc")); // Soft slate-50 background

        var typeface = FontResolver.GetDefaultTypeface();
        using var titleFont = new SKFont(typeface, 24);
        using var subFont = new SKFont(typeface, 13);
        using var labelFont = new SKFont(typeface, 13);
        using var pctFont = new SKFont(typeface, 13);
        using var centerFont = new SKFont(typeface, 20);
        using var centerSubFont = new SKFont(typeface, 11);

        using var boldPaint = new SKPaint { Color = SKColor.Parse("#0f172a"), IsAntialias = true };
        using var textPaint = new SKPaint { Color = SKColor.Parse("#1e293b"), IsAntialias = true };
        using var mutedPaint = new SKPaint { Color = SKColor.Parse("#64748b"), IsAntialias = true };

        // Outer Card Container
        using var cardFill = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };
        using var cardBorder = new SKPaint { Color = SKColor.Parse("#e2e8f0"), IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };
        var cardRect = new SKRoundRect(new SKRect(30, 30, width - 30, height - 30), 16, 16);
        canvas.DrawRoundRect(cardRect, cardFill);
        canvas.DrawRoundRect(cardRect, cardBorder);

        // Header Section
        canvas.DrawText("Employee Work Distribution (Percentage)", 60, 80, SKTextAlign.Left, titleFont, boldPaint);
        canvas.DrawText("Proportion of total company time contributed by each team member", 60, 106, SKTextAlign.Left, subFont, mutedPaint);

        if (employees == null || employees.Count == 0)
        {
            canvas.DrawText("No data available to display.", 60, 200, SKTextAlign.Left, labelFont, mutedPaint);
            SaveBitmap(bitmap, outputFilePath);
            return;
        }

        double totalHours = employees.Sum(e => e.TotalHoursWorked);
        if (totalHours <= 0)
        {
            canvas.DrawText("Total worked hours is zero.", 60, 200, SKTextAlign.Left, labelFont, mutedPaint);
            SaveBitmap(bitmap, outputFilePath);
            return;
        }

        // Pie Circle Geometry
        float centerX = 290;
        float centerY = 390;
        float radius = 190;
        var pieRect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);

        float currentAngle = -90f; // Start from top
        using var slicePaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
        using var sliceBorder = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.White, StrokeWidth = 2.5f };

        // Legend Coordinates
        float legendStartX = 560;
        float legendStartY = 150;
        float boxSize = 14;
        float rowSpacing = Math.Min(38f, 480f / Math.Max(1, employees.Count));

        for (int i = 0; i < employees.Count; i++)
        {
            var emp = employees[i];
            float sweepAngle = (float)((emp.TotalHoursWorked / totalHours) * 360.0);
            var color = SKColor.Parse(PaletteHex[i % PaletteHex.Length]);

            // Draw Pie Slice
            slicePaint.Color = color;
            var pb = new SKPathBuilder();
            pb.MoveTo(centerX, centerY);
            pb.ArcTo(pieRect, currentAngle, sweepAngle, false);
            pb.Close();
            using var path = pb.Detach();

            canvas.DrawPath(path, slicePaint);
            canvas.DrawPath(path, sliceBorder);
            currentAngle += sweepAngle;

            // Draw Legend Row
            float rowY = legendStartY + (i * rowSpacing);
            var colorBox = new SKRoundRect(new SKRect(legendStartX, rowY, legendStartX + boxSize, rowY + boxSize), 3, 3);
            canvas.DrawRoundRect(colorBox, slicePaint);

            // Name
            canvas.DrawText(emp.EmployeeName, legendStartX + boxSize + 12, rowY + 12, SKTextAlign.Left, labelFont, textPaint);

            // Hours & Percentage
            string hoursStr = $"{emp.TotalHoursWorked:F2} hrs";
            string pctStr = $"{emp.PercentageOfTotal:F2}%";

            canvas.DrawText(hoursStr, legendStartX + 250, rowY + 12, SKTextAlign.Left, pctFont, mutedPaint);
            canvas.DrawText(pctStr, legendStartX + 350, rowY + 12, SKTextAlign.Left, pctFont, boldPaint);
        }

        // Inner Donut Cutout (Modern Infographic Style)
        float innerRadius = 85;
        var innerRect = new SKRect(centerX - innerRadius, centerY - innerRadius, centerX + innerRadius, centerY + innerRadius);
        using var holePaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Fill };
        canvas.DrawOval(innerRect, holePaint);
        canvas.DrawOval(innerRect, cardBorder);

        // Donut Center Text
        string centerTotal = $"{totalHours:F0} hrs";
        string centerTeam = $"{employees.Count} Members";

        canvas.DrawText(centerTeam, centerX, centerY - 6, SKTextAlign.Center, centerSubFont, mutedPaint);
        canvas.DrawText(centerTotal, centerX, centerY + 20, SKTextAlign.Center, centerFont, boldPaint);

        SaveBitmap(bitmap, outputFilePath);
    }

    private static void SaveBitmap(SKBitmap bitmap, string outputFilePath)
    {
        string? directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var dataStream = image.Encode(SKEncodedImageFormat.Png, 100);
        using var fileStream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write);
        dataStream.SaveTo(fileStream);
    }
}
