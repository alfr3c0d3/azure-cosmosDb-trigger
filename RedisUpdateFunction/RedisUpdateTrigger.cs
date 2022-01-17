using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Services;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger
{
    public class RedisUpdateTrigger
    {
        private readonly IVehicleService _vehicleService;

        private const string FunctionName = "ShopNotesRedisUpdateTrigger";

        public RedisUpdateTrigger(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [FunctionName(FunctionName)]
        public async Task Run([CosmosDBTrigger(
            databaseName: "Tasks",
            collectionName: "Vehicles",
            ConnectionStringSetting = "poc-alfre_DOCUMENTDB",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> insertedRecords, ILogger logger)
        {
            if (insertedRecords == null || !insertedRecords.Any())
            {
                logger.LogInformation($"{FunctionName}: No records inserted");
                return;
            }

            // Update Redis Cache with new Inserted records
            await _vehicleService.UpdateRedisCache(insertedRecords, FunctionName);
        }
    }
}
