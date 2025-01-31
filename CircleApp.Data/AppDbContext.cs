﻿using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CircleApp.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>().HasMany(u => u.Posts).WithOne(p => p.User).HasForeignKey(p => p.UserId);

            // Like
            modelBuilder.Entity<Like>().HasKey(l => new { l.PostId, l.UserId});
            modelBuilder.Entity<Like>().HasOne(l => l.Post).WithMany(p => p.Likes).HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Like>().HasOne(l => l.User).WithMany(u => u.Likes).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Restrict);

            // Comment
            modelBuilder.Entity<Comment>().HasOne(l => l.Post).WithMany(p => p.Comments).HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Comment>().HasOne(l => l.User).WithMany(u => u.Comments).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Restrict);

            // Favorite
            modelBuilder.Entity<Favorite>().HasKey(f => new {f.PostId, f.UserId});
            modelBuilder.Entity<Favorite>().HasOne(f => f.Post).WithMany(p => p.Favorites).HasForeignKey(f => f.PostId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Favorite>().HasOne(f => f.User).WithMany(u => u.Favorites).HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Restrict);

            // Report
            modelBuilder.Entity<Report>().HasKey(r => new { r.PostId, r.UserId });
            modelBuilder.Entity<Report>().HasOne(r => r.Post).WithMany(p => p.Reports).HasForeignKey(r => r.PostId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Report>().HasOne(r => r.User).WithMany(u => u.Reports).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

        }
    }
}
