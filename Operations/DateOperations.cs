﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Operations {
    public static class DateOperations {
        public static DateTime Today() => Now().Date;
        public static DateTime Now() => DateTime.UtcNow;
        public static void GetTodayTimeSlotsBoundsUtc(int timeSlotSpan, out DateTime start, out DateTime end) => GetTimeSlotsBoundsUtc(timeSlotSpan, Today().Year, Today().Month, Today().Day, out start, out end);

        public static void GetTimeSlotsBoundsUtc(int timeSlotSpan, int year, int month, int day, out DateTime start, out DateTime end) {
            start = new DateTime(year, month, day).ToUniversalTime();
            end = start.AddDays(1).AddMinutes(timeSlotSpan);
        }

        public static bool IsWithinBounds(this DateTime date, DateTime start, DateTime end) {
            return date.CompareTo(start) >= 0 && date.CompareTo(end) <= 0;
        }
    }
}
