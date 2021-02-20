using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;
using uul_api.Operations;

namespace uul_api.Data {
    public class TimeSlotsFactory {
     
        public static async Task<List<TimeSlot>> CreateTodayTimeSlots(UULContext context, int hourToStart) {
            var rules = await RulesDao.GetCurrentRulesDTO(context);
            DateOperations.GetTodayTimeSlotsBoundsUtc(rules, out DateTime start, out DateTime end);
            var existent = await TimeSlotsDao.GetTimeSlotsByUtcBounds(context, start, end);
            if (existent.Count != 0) {
                return new List<TimeSlot>();
            }
            var today = DateTime.Today.ToUniversalTime();
            var limit = DateTime.Today.AddDays(1).ToUniversalTime();
            var slotStart = today.AddHours(hourToStart);
            var slots = new List<TimeSlot>();
            var slotSpan = TimeSpan.FromMinutes(rules.TimeSlotSpan);
            while (slotStart.CompareTo(limit) < 0) {
                var slot = new TimeSlot {
                    Start = slotStart.ToUniversalTime(),
                    End = (slotStart + slotSpan).ToUniversalTime()
                };
                slots.Add(slot);
                slotStart += slotSpan;
            }
            return slots;
        }

        public static async Task<List<TimeSlot>> CreateTimeSlotsForDateUTC(UULContext context, DateTime dateUtc, int hourToStart) {
            var rules = await RulesDao.GetCurrentRulesDTO(context);
            DateOperations.GetTimeSlotsBoundsUtc(rules, dateUtc.Year, dateUtc.Month, dateUtc.Hour, out DateTime start, out DateTime end);
            var existent = await TimeSlotsDao.GetTimeSlotsByUtcBounds(context, start, end);
            if (existent.Count != 0) {
                return new List<TimeSlot>();
            }
            var limit = dateUtc.AddDays(1);
            var slotStart = dateUtc.AddHours(hourToStart);
            var slots = new List<TimeSlot>();
            var slotSpan = TimeSpan.FromMinutes(rules.TimeSlotSpan);
            while (slotStart.CompareTo(limit) < 0) {
                var slot = new TimeSlot {
                    Start = slotStart.ToUniversalTime(),
                    End = (slotStart + slotSpan).ToUniversalTime()
                };
                slots.Add(slot);
                slotStart += slotSpan;
            }
            return slots;
        }
    }
}
