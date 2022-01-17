using Microsoft.Azure.Documents;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Services
{
    public interface IVehicleService
    {
        Task<ICollection<VehicleShopNoteDto>> GetVehicleShopNotes(IEnumerable<string> vehicleNumbers);
        Task UpdateRedisCache(IReadOnlyList<Document> insertedDocuments, string functionName);
    }
}
