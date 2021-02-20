using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data.Dummy {
    public class DummyDataFactory {
        public static void CreateDummyData(UULContext context) {
       
            for (int i = 0; i < 20; i++) {
                var user = new NewUserDTO() { Login = "Login" + i, AvatarSrc = "Default", Name = "Inhabitant_" + i, ApartmentCode = "T" + i, Pwd = "12345" };
                UserDao.AddFromDto(context, user);
            }
            context.SaveChanges();
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            var today = DateTime.UtcNow;

            for (int i = 0; i < 3; i++) {
                var slots = TimeSlotsFactory.CreateTimeSlotsForDateUTC(context, new DateTime(today.Year, today.Month, today.Day - i).ToUniversalTime(), 5);
                slots.Wait();
                timeSlots.AddRange(slots.Result);
            }
            var habitants = context.Habitants.ToList();
            var size = habitants.Count;
            var rnd = new Random();
            foreach (TimeSlot timeSlot in timeSlots) {
                for(int i = 0; i < rnd.Next(4); i++) {
                    if (timeSlot.OccupiedBy == null) { timeSlot.OccupiedBy = new List<Habitant>(); }
                    timeSlot.OccupiedBy.Add(habitants.ElementAt(rnd.Next(size)));
                }
            }
            context.TimeSlots.AddRange(timeSlots);
            context.SaveChanges();

        }
    }
}
