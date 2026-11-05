using System;
using System.Text.Json.Serialization;

namespace EmployeeTimeVisualizer.Models;

/// <summary>
/// Represents a raw time entry returned by the Azure Function API.
/// </summary>
public class TimeEntry
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("EmployeeName")]
    public string? EmployeeName { get; set; }

    /// <summary>
    /// Note: The Azure Function API returns this property with the key 'StarTimeUtc' (spelled without 't').
    /// </summary>
    [JsonPropertyName("StarTimeUtc")]
    public DateTime? StartTimeUtc { get; set; }

    [JsonPropertyName("EndTimeUtc")]
    public DateTime? EndTimeUtc { get; set; }

    [JsonPropertyName("EntryNotes")]
    public string? EntryNotes { get; set; }

    [JsonPropertyName("DeletedOn")]
    public DateTime? DeletedOn { get; set; }

    /// <summary>
    /// Calculates the non-negative duration in hours for this entry.
    /// Returns 0 if timestamps are missing, deleted, or inverted.
    /// </summary>
    public double GetDurationHours()
    {
        if (!StartTimeUtc.HasValue || !EndTimeUtc.HasValue)
        {
            return 0;
        }

        if (EndTimeUtc.Value <= StartTimeUtc.Value)
        {
            return 0;
        }

        return (EndTimeUtc.Value - StartTimeUtc.Value).TotalHours;
    }
}
