﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;
using uul_api.Operations;

namespace uul_api.Data {
    public class TimeSlotsFactory {
     
        public static Task<List<TimeSlot>> CreateTodayTimeSlots(UULContext context, int hourToStart) => CreateTimeSlotsForDateUTC(context, DateOperations.Today(), hourToStart);

        public static async Task<List<TimeSlot>> CreateTimeSlotsForDateUTC(UULContext context, DateTime dateUtc, int hourToStart) {
            var rules = await RulesDao.GetCurrentRulesOrDefault(context);
            DateOperations.GetTimeSlotsBoundsUtc(rules.TimeSlotSpan, dateUtc.Year, dateUtc.Month, dateUtc.Day, out DateTime start, out DateTime end);
            var existent = await TimeSlotsDao.GetTimeSlotsByUtcBounds(context, start, end);
            if (existent.Count != 0) {
                return new List<TimeSlot>();
            }
            var limit = dateUtc.AddDays(1);
            var slotStart = dateUtc.AddHours(hourToStart);
            var slots = new List<TimeSlot>();
            var slotSpan = TimeSpan.FromMinutes(rules.TimeSlotSpan);
            while (slotStart.CompareTo(limit) < 0) {
                foreach (Gym gym in rules.Gyms) {
                    var slot = new TimeSlot {
                        Start = slotStart.ToUniversalTime(),
                        End = (slotStart + slotSpan).ToUniversalTime(),
                        Gym = gym
                    };
                    slots.Add(slot);
                }
                slotStart += slotSpan;
            }
            return slots;
        }
    }
}
