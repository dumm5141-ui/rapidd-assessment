using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Service interface for retrieving raw time entry records.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Fetches time entries from the remote Azure Function endpoint.
    /// </summary>
    Task<IReadOnlyList<TimeEntry>> FetchTimeEntriesAsync(CancellationToken cancellationToken = default);
}
