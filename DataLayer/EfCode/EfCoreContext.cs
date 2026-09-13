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

            // Unique Indexes for semaphore implementation (concurrent requests)
            // The unique indexes will prevent multiple entries with the same value,
            // which can be used to implement a semaphore mechanism for concurrent requests.
            // For example, if you want to ensure that only one request is processing a specific search key
            // at a time, you can create a unique index on the Title property of the SearchKey entity.
            // This way, if another request tries to insert a SearchKey with the same Title
            // while the first request is still processing, it will fail due to the unique constraint,
            // effectively acting as a semaphore.
            modelBuilder.Entity<Channel>()  
                .HasIndex(k => k.ChId)
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasIndex(k => k.CId)
                .IsUnique();

            //modelBuilder.Entity<History>()
            //    .HasIndex(k => k.UserId)
            //    .IsUnique();

            //modelBuilder.Entity<History_VideoWatched>()
            //    .HasIndex(k => k.HistoryId+k.VideoWatchedId)
            //    .IsUnique();

            modelBuilder.Entity<LiveChatComment>()
                .HasIndex(k => new { k.CId, k.PageNum })
                .IsUnique();

            modelBuilder.Entity<SearchKey>() 
                .HasIndex(k => k.Title)
                .IsUnique();

            modelBuilder.Entity<SearchResultedVideo>() 
                .HasIndex(k => new { k.VId, k.PageNum })
                .IsUnique();

            modelBuilder.Entity<SearchSuggestions>() 
                .HasIndex(k => k.Title)
                .IsUnique();

            modelBuilder.Entity<User>() 
                .HasIndex(k => k.UId)
                .IsUnique();

            modelBuilder.Entity<Video>() 
                .HasIndex(k => new { k.VId,k.PageNum })
                .IsUnique();

            modelBuilder.Entity<VideoCategory>() 
                .HasIndex(k => k.VCId)
                .IsUnique();

            modelBuilder.Entity<VideoWatched>() 
                .HasIndex(k => k.VId)
                .IsUnique();

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
