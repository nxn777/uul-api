using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class TimeSlotsDao {
        public static Task<List<TimeSlotDTO>> GetTimeSlotsByUtcBounds(UULContext context, DateTime start, DateTime end) {
            return context.TimeSlots
                .Where(ts => ts.Start.CompareTo(start) >= 0 && ts.End.CompareTo(end) < 0)
                .OrderBy(ts => ts.Start)
                .Include(slot => slot.OccupiedBy)
                .ThenInclude(h => h.User)
                .Select(e => e.ToDTO())
                .ToListAsync();
        }


    }
}
