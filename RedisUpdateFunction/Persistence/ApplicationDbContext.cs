using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Entities;
using System.Collections.Generic;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Persistence
{
    internal class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<VehicleShopNote> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VehicleShopNote>(entity =>
            {
                entity
                .HasNoDiscriminator()
                .ToContainer("Vehicles")
                .Property(e => e.Id).ToJsonProperty("id");

                entity
                    .Property(e => e.TaskIds)
                    .HasConversion(t => JsonConvert.SerializeObject(t),
                        t => JsonConvert.DeserializeObject<IList<long>>(t));

                entity.HasKey(e => new { e.Id, e.VehicleNumber });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
