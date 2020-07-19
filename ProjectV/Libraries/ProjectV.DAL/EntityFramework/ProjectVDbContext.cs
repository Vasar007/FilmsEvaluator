﻿using System;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ProjectV.DAL.EntityFramework
{
    public sealed class ProjectVDbContext : DbContext
    {
        private readonly DataBaseOptions _storageOptions;

        public DbSet<TaskDbModel>? Tasks { private get; set; }


        public ProjectVDbContext(DataBaseOptions storageOptions)
        {
            _storageOptions = storageOptions.ThrowIfNull(nameof(storageOptions));
        }

        public ProjectVDbContext(IOptions<DataBaseOptions> storageOptions)
        {
            _storageOptions = storageOptions.Value.ThrowIfNull(nameof(storageOptions));
        }

        public DbSet<TaskDbModel> GetTasksDbSet()
        {
            if (Tasks is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Tasks)} DB set is not initialized."
                );
            }

            return Tasks;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_storageOptions.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);

            // Key fields have already mapped. Need to set the other fields.
            modelBuilder.Entity<TaskDbModel>(
                builder =>
                {
                    builder.HasKey(e => e.Id);
                    builder.Property(e => e.Name);
                    builder.Property(e => e.State);
                    builder.Property(e => e.Result);
                    builder.Property(e => e.Config);
                }
            );
        }
    }
}
