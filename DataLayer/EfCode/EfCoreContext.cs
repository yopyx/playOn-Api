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
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
namespace DataLayer.EfCode
{
    public class EfCoreContext(DbContextOptions<EfCoreContext> options) : DbContext(options), IDataProtectionKeyContext
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
                .Property(c => c.Tags)
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

            modelBuilder.Entity<SearchResultedVideo>()
                .Property(c => c.Tags)
                .HasJsonConversion(options);
            modelBuilder.Entity<SearchResultedVideo>()
                .Property(c => c.Thumbnails)
                .HasJsonConversion(options);
            modelBuilder.Entity<SearchResultedVideo>()
                .Property(c => c.Statistics)
                .HasJsonConversion(options);
            modelBuilder.Entity<SearchResultedVideo>()
                .Property(c => c.LiveStreamingDetails)
                .HasJsonConversion(options);

            modelBuilder.Entity<SearchSuggestions>()
                .Property(c => c.Suggestions)
                .HasJsonConversion(options);

            modelBuilder.Entity<VideoWatched>()
                .Property(c => c.Tags)
                .HasJsonConversion(options);
            modelBuilder.Entity<VideoWatched>()
                .Property(c => c.Thumbnails)
                .HasJsonConversion(options);
            modelBuilder.Entity<VideoWatched>()
                .Property(c => c.Statistics)
                .HasJsonConversion(options);
            modelBuilder.Entity<VideoWatched>()
                .Property(c => c.LiveStreamingDetails)
                .HasJsonConversion(options);
        }

        public DbSet<VideoCategory> VideoCategories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<SearchKey> SearchKeys { get; set; }
        public DbSet<SearchResultedVideo> VideoResults { get; set; }
        public DbSet<SearchSuggestions> SearchSuggestions { get; set; }
        public DbSet<VideoWatched> VideosWatched { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LiveChatComment> LiveComments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<History_VideoWatched> VideosHistory { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}
