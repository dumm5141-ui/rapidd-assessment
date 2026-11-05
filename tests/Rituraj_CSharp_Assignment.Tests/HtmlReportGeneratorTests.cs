using System.Collections.Generic;
using Rituraj_CSharp_Assignment.Models;
using Rituraj_CSharp_Assignment.Services;
using Xunit;

namespace Rituraj_CSharp_Assignment.Tests;

public class HtmlReportGeneratorTests
{
    private readonly HtmlReportGenerator _generator = new();

    [Fact]
    public void GenerateHtml_ShouldIncludeRequiredHeadersAndColorLowHoursRow()
    {
        // Arrange
        var employees = new List<EmployeeSummary>
        {
            new()
            {
                EmployeeName = "High Performer",
                TotalHoursWorked = 150.00,
                EntryCount = 20,
                PercentageOfTotal = 60.0
            },
            new()
            {
                EmployeeName = "Low Hours Employee",
                TotalHoursWorked = 75.50,
                EntryCount = 10,
                PercentageOfTotal = 40.0
            }
        };

        // Act
        string html = _generator.GenerateHtml(employees, pieChartRelativePath: "piechart.png");

        // Assert
        // Check document structure
        Assert.Contains("<!DOCTYPE html>", html);
        Assert.Contains("<table", html);
        Assert.Contains("<th>Name</th>", html);
        Assert.Contains("<th>Total Time Worked</th>", html);

        // Check content
        Assert.Contains("High Performer", html);
        Assert.Contains("150.00 hrs", html);
        Assert.Contains("Low Hours Employee", html);
        Assert.Contains("75.50 hrs", html);

        // Check threshold styling: low hours row MUST contain class="low-hours"
        Assert.Contains("<tr class=\"low-hours\">", html);
        Assert.Contains("&lt; 100 Hours", html);

        // Check image embedding
        Assert.Contains("<img src=\"piechart.png\"", html);
    }

    [Fact]
    public void GenerateHtml_WhenAllEmployeesAbove100Hours_ShouldNotHaveLowHoursRows()
    {
        // Arrange
        var employees = new List<EmployeeSummary>
        {
            new()
            {
                EmployeeName = "Worker One",
                TotalHoursWorked = 120.00,
                EntryCount = 15,
                PercentageOfTotal = 50.0
            },
            new()
            {
                EmployeeName = "Worker Two",
                TotalHoursWorked = 120.00,
                EntryCount = 15,
                PercentageOfTotal = 50.0
            }
        };

        // Act
        string html = _generator.GenerateHtml(employees);

        // Assert
        Assert.DoesNotContain("class=\"low-hours\"", html);
        Assert.Contains("Compliant", html);
    }
}
