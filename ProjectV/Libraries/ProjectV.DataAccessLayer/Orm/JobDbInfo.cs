﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;

namespace ProjectV.DataAccessLayer.Orm
{
    [Table("jobs")]
    public sealed class JobDbInfo
    {
        [Key, Required]
        [Column("id")]
        public Guid Id { get; private set; }

        [Required]
        [Column("name")]
        public string Name { get; private set; }

        [Required]
        [Column("state")]
        public int State { get; private set; }

        [Required]
        [Column("result")]
        public int Result { get; private set; }

        [Required]
        [Column("config")]
        public string Config { get; private set; }


        // For LinqToDb only.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private JobDbInfo()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public JobDbInfo(
            Guid id,
            string name,
            int state,
            int result,
            string config)
        {
            Id = id;
            Name = name.ThrowIfNullOrWhiteSpace(nameof(config));
            State = state;
            Result = result;
            Config = config.ThrowIfNullOrWhiteSpace(nameof(config));
        }
    }
}
