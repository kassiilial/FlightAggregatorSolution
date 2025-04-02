using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FlightAggregator.Providers;

public class CacheHelper(IDistributedCache cache,
    ILogger<CacheHelper> logger)
{
    public string GetCacheKey(string providerName, string departure, string destination, DateTime date)
    {
        return $"{providerName}-flights-{departure}-{destination}-{date:yyyyMMdd}";
    }
        
    public async Task<List<Flight>?> GetFlightsFromCacheAsync(string cacheKey, CancellationToken cancellationToken)
    {
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                return JsonSerializer.Deserialize<List<Flight>>(cachedData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка десериализации данных из кэша по ключу {CacheKey}", cacheKey);
            }
        }
        return null;
    }
        
    public async Task SetFlightsToCacheAsync(string cacheKey, List<Flight> flights, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        var data = JsonSerializer.Serialize(flights);
        await cache.SetStringAsync(cacheKey, data, options, cancellationToken);
    }
}