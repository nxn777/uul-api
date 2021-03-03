﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public static class NewsDao {

        public static async Task<ICollection<News>> GetNewsAsync(UULContext context, Auditory auditory) {
            return await context.News.Where(n => (int)n.Auditory <= (int) auditory).ToListAsync();
        }

    }
}
