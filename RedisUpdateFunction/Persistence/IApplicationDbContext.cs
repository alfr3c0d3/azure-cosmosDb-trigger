using Microsoft.EntityFrameworkCore;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Entities;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Persistence
{
    internal interface IApplicationDbContext
    {
        public DbSet<VehicleShopNote> Vehicles { get; set; }
    }
}