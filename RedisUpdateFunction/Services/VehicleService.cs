using AutoMapper;
using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Dtos;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Extensions;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Services
{
    internal class VehicleService : IVehicleService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        private const string RedisKey = "list_by_vehicle_";

        public VehicleService(IApplicationDbContext context, IDistributedCache cache, IMapper mapper, IConfiguration configuration, ILogger<VehicleService> logger)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ICollection<VehicleShopNoteDto>> GetVehicleShopNotes(IEnumerable<string> vehicleNumbers)
        {
            var dbRecords = await _context.Vehicles.Where(e => vehicleNumbers.Contains(e.VehicleNumber))?.ToListAsync();

            return _mapper.Map<ICollection<VehicleShopNoteDto>>(dbRecords);
        }

        public async Task UpdateRedisCache(IReadOnlyList<Document> insertedDocuments, string functionName)
        {
            // Get the inserted Vehicle Numbers
            var dbRecords = _mapper.Map<IEnumerable<VehicleShopNoteDto>>(insertedDocuments);

            if (dbRecords == null || !dbRecords.Any()) return;

            // Group shopNotes by VehicleNumber
            var shopNotes = dbRecords
                .GroupBy(x => x.VehicleNumber)
                .Select(x => new
                {
                    VehicleNumber = x.Key,
                    ShopNotes = x.Select(x => x)
                });

            foreach (var shopNote in shopNotes)
            {
                var redisKey = $"{RedisKey}{shopNote.VehicleNumber}";

                // Retrieve from REDIS by Vehicle Number
                var redisCache = await _cache.GetRecordAsync<List<VehicleShopNoteDto>>(redisKey) /*?? new List<VehicleShopNoteDto>()*/;
                redisCache.AddRange(shopNote.ShopNotes);

                try
                {
                    // Save new Records to Redis 
                    await _cache.SetRecordAsync(redisKey, redisCache);

                    // Create Log Json
                    var log = new
                    {
                        DbNumber = _configuration.GetConnectionString("Redis:DataBase"),
                        Key = $"shop_notes_{redisKey}",
                        Data = redisCache
                    };

                    _logger.LogInformation($"{functionName}: Saved {JsonConvert.SerializeObject(log)}");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"{functionName}: Failed to save shop_notes_{redisKey}");
                    _logger.LogError(ex.Message, ex);
                }
            }
        }
    }
}
