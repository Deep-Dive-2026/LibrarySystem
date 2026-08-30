using LibrarySystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace LibrarySystem.Application.Queries.Books.GetTopBooks
{
    public class GetTopBooksQueryHandler
        : IRequestHandler<GetTopBooksQuery, IReadOnlyList<TopBooksDto>>
    {
        private readonly ILogger<GetTopBooksQueryHandler> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBookRepository _bookRepository;
        private readonly IDistributedCache _redisCache;
        private readonly HybridCache _hybridCache;

        public GetTopBooksQueryHandler(IBookRepository bookRepository, IDistributedCache redisCache,
            IMemoryCache cache, HybridCache hybridCache, ILogger<GetTopBooksQueryHandler> logger)
        {
            _cache = cache;
            _bookRepository = bookRepository;
            _redisCache = redisCache;
            _hybridCache = hybridCache;
            _logger = logger;
        }
        public async Task<IReadOnlyList<TopBooksDto>> Handle(GetTopBooksQuery request, CancellationToken cancellationToken)
        {
            //return await _bookRepository.GetTopBooksAsync(cancellationToken);


            #region IMemory Cache
            //const string cacheKey = "top-books";

            //bool cacheMiss = false;

            //var topBooks = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            //    {
            //        cacheMiss = true;

            //        var result = await _bookRepository.GetTopBooksAsync(cancellationToken);

            //        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);


            //        _logger.LogInformation("Returned from DATABASE");

            //        return result;
            //    });

            //if (!cacheMiss)
            //{
            //    _logger.LogInformation("Returned From Memory CACHE");
            //}


            //return topBooks!;
            #endregion


            #region Redis Cache

            //var cached = await _redisCache.GetStringAsync("dashboard:top-books", cancellationToken);

            //if (cached is not null)
            //{
            //    _logger.LogInformation("Returned From Redis Cache");
            //    return JsonSerializer.Deserialize<List<TopBooksDto>>(cached)!;
            //}

            //var topBooks = await _bookRepository.GetTopBooksAsync(cancellationToken);

            //await _redisCache.SetStringAsync("dashboard:top-books", JsonSerializer.Serialize(topBooks),
            //    new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            //    }, cancellationToken);

            //_logger.LogInformation("Returned From Database");
            //return topBooks;
            #endregion


            #region Hybrid Cache
            const string cacheKey = "top-books";

            bool cacheMiss = false;

            var topBooks = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                cacheMiss = true;

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);


                var result = await _bookRepository.GetTopBooksAsync(cancellationToken);
                _logger.LogInformation("Returned from DATABASE");

                return result;
            });

            if (!cacheMiss)
            {
                _logger.LogInformation("Returned From Hybrid CACHE");
            }


            return topBooks!;
            #endregion

        }
    }
}
