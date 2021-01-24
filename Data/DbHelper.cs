using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class DbHelper {
        public static Task<List<TimeSlot>> GetTodayTimeSlots(UULContext context) {
            var today = DateTime.Today;
            return context.TimeSlots.Where(t => t.Start.CompareTo(today) >= 0 && t.End.CompareTo(today) < 0).ToListAsync();
        }

        public static async Task<List<TimeSlot>> CreateTodayTimeSlots(UULContext context, TimeSpan slotSpan, int hourToStart) {
            var existent = await GetTodayTimeSlots(context);
            if (existent.Count != 0) {
                return new List<TimeSlot>();
            }
            var today = DateTime.Today.ToUniversalTime();
            var limit = DateTime.Today.AddDays(1).ToUniversalTime();
            var slotStart = today.AddHours(hourToStart);
            var slots = new List<TimeSlot>();
            while (slotStart.CompareTo(limit) < 0) {
                var slot = new TimeSlot {
                    Start = slotStart,
                    End = slotStart + slotSpan
                };
                slots.Add(slot);
                slotStart += slotSpan;
            }
            return slots;
        }
    }
}
