using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext(DbContextOptions<EfCoreContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var options = new JsonSerializerOptions();

            modelBuilder.Entity<Channel>()
                .Property(c => c.Thumbnails)
                .HasJsonConversion(options);
            modelBuilder.Entity<Channel>()
                .Property(c => c.Statistics)
                .HasJsonConversion(options);

            modelBuilder.Entity<Video>()
                .Property(c => c.Thumbnails)
                .HasJsonConversion(options);
            modelBuilder.Entity<Video>()
                .Property(c => c.Statistics)
                .HasJsonConversion(options);
            modelBuilder.Entity<Video>()
                .Property(c => c.LiveStreamingDetails)
                .HasJsonConversion(options);
        }

        public DbSet<VideoCategory> VideoCategories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}
