﻿using CommerceFlow.Persistence.Configuration;
using CommerceFlow.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace CommerceFlow.Persistence
{
    public class CommerceDbContext(DbContextOptions<CommerceDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityByDefaultColumns();

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
