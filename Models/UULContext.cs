﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace uul_api.Models {
    public class UULContext : DbContext {
        public UULContext(DbContextOptions<UULContext> options)
            : base(options) {
        }

        public DbSet<Appartment> Appartments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }

    }
}

