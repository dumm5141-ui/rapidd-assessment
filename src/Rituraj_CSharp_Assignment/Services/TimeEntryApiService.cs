using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Rituraj_CSharp_Assignment.Models;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Default implementation of IApiService using HttpClient to fetch entries from Azure Functions.
/// </summary>
public class TimeEntryApiService : IApiService
{
    public const string DefaultEndpointUrl =
        "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public TimeEntryApiService(HttpClient? httpClient = null, string? endpointUrl = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _endpointUrl = string.IsNullOrWhiteSpace(endpointUrl) ? DefaultEndpointUrl : endpointUrl;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IReadOnlyList<TimeEntry>> FetchTimeEntriesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(_endpointUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var entries = await JsonSerializer.DeserializeAsync<List<TimeEntry>>(contentStream, _jsonOptions, cancellationToken);

        return entries ?? new List<TimeEntry>();
    }
}
