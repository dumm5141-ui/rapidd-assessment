using System;
using System.Collections.Generic;
using System.Linq;
using Rituraj_CSharp_Assignment.Models;
using Rituraj_CSharp_Assignment.Services;
using Xunit;

namespace Rituraj_CSharp_Assignment.Tests;

public class EmployeeReportServiceTests
{
    private readonly EmployeeReportService _service = new();

    [Fact]
    public void ProcessEntries_ShouldSumHoursAndSortDescending()
    {
        // Arrange
        var entries = new List<TimeEntry>
        {
            new()
            {
                EmployeeName = "Alice",
                StartTimeUtc = new DateTime(2023, 1, 1, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 1, 17, 0, 0) // 8 hours
            },
            new()
            {
                EmployeeName = "Bob",
                StartTimeUtc = new DateTime(2023, 1, 1, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 1, 19, 0, 0) // 10 hours
            },
            new()
            {
                EmployeeName = "Alice",
                StartTimeUtc = new DateTime(2023, 1, 2, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 2, 13, 0, 0) // 4 hours
            }
        };

        // Act
        var result = _service.ProcessEntries(entries);

        // Assert
        Assert.Equal(2, result.Count);
        // Alice total = 12h, Bob total = 10h -> Alice should be first
        Assert.Equal("Alice", result[0].EmployeeName);
        Assert.Equal(12.0, result[0].TotalHoursWorked);
        Assert.Equal(2, result[0].EntryCount);

        Assert.Equal("Bob", result[1].EmployeeName);
        Assert.Equal(10.0, result[1].TotalHoursWorked);
        Assert.Equal(1, result[1].EntryCount);

        // Percentages: total 22h -> Alice = 12/22 (54.55%), Bob = 10/22 (45.45%)
        Assert.Equal(54.55, result[0].PercentageOfTotal);
        Assert.Equal(45.45, result[1].PercentageOfTotal);
    }

    [Fact]
    public void ProcessEntries_ShouldExcludeDeletedRecords()
    {
        // Arrange
        var entries = new List<TimeEntry>
        {
            new()
            {
                EmployeeName = "Raja",
                StartTimeUtc = new DateTime(2023, 1, 1, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 1, 17, 0, 0), // 8h
                DeletedOn = null
            },
            new()
            {
                EmployeeName = "Raja",
                StartTimeUtc = new DateTime(2023, 1, 2, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 2, 17, 0, 0), // 8h
                DeletedOn = new DateTime(2023, 1, 3, 0, 0, 0) // Deleted!
            }
        };

        // Act
        var result = _service.ProcessEntries(entries);

        // Assert
        Assert.Single(result);
        Assert.Equal("Raja", result[0].EmployeeName);
        Assert.Equal(8.0, result[0].TotalHoursWorked);
        Assert.Equal(1, result[0].EntryCount);
    }

    [Fact]
    public void ProcessEntries_ShouldIgnoreInvertedOrNegativeTimes()
    {
        // Arrange
        var entries = new List<TimeEntry>
        {
            new()
            {
                EmployeeName = "Monica",
                StartTimeUtc = new DateTime(2023, 1, 1, 17, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 1, 9, 0, 0) // End before Start (negative)
            },
            new()
            {
                EmployeeName = "Monica",
                StartTimeUtc = new DateTime(2023, 1, 2, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 2, 14, 0, 0) // 5h valid
            }
        };

        // Act
        var result = _service.ProcessEntries(entries);

        // Assert
        Assert.Single(result);
        Assert.Equal("Monica", result[0].EmployeeName);
        Assert.Equal(5.0, result[0].TotalHoursWorked);
    }

    [Fact]
    public void ProcessEntries_ShouldApplyRealisticFallbackForMissingNames()
    {
        // Arrange
        var entries = new List<TimeEntry>
        {
            new()
            {
                EmployeeName = null,
                StartTimeUtc = new DateTime(2023, 1, 1, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 1, 15, 0, 0) // 6h
            },
            new()
            {
                EmployeeName = "   ",
                StartTimeUtc = new DateTime(2023, 1, 2, 9, 0, 0),
                EndTimeUtc = new DateTime(2023, 1, 2, 13, 0, 0) // 4h
            }
        };

        // Act
        var result = _service.ProcessEntries(entries, unassignedFallbackName: "Vijay (Unassigned)");

        // Assert
        Assert.Single(result);
        Assert.Equal("Vijay (Unassigned)", result[0].EmployeeName);
        Assert.Equal(10.0, result[0].TotalHoursWorked);
        Assert.True(result[0].IsUnderThreshold);
    }

    [Theory]
    [InlineData(99.99, true)]
    [InlineData(100.0, false)]
    [InlineData(150.5, false)]
    public void EmployeeSummary_IsUnderThreshold_ShouldCorrectlyReflect100HoursRule(double hours, bool expectedUnderThreshold)
    {
        var summary = new EmployeeSummary
        {
            EmployeeName = "Test Employee",
            TotalHoursWorked = hours
        };

        Assert.Equal(expectedUnderThreshold, summary.IsUnderThreshold);
    }
}
