using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Data.Dummy;
using uul_api.Models;
using uul_api.Security;

namespace uul_api.Data {
    public class DBInitializer {
        private const int DefaultTimeSlotSpan = 60;
        public static void Initialize(UULContext context) {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            if (!context.Users.Any()) {
                context.Users.Add(SecHelper.CreateDefaultAdmin());
            }
            if (!context.Rules.Any()) {
                context.Towers.RemoveRange(context.Towers);
                context.SpecialFloors.RemoveRange(context.SpecialFloors);
                context.BannedApartments.RemoveRange(context.BannedApartments);
                context.Gyms.RemoveRange(context.Gyms);

                var towers = new List<Tower>() { 
                    new Tower() { Name = "A", FloorsCount = 10 }, 
                    new Tower() { Name = "B", FloorsCount = 10 },
                    new Tower() { Name = "C", FloorsCount = 12 },
                    new Tower() { Name = "D", FloorsCount = 12 }
                };

                var specialFloors = new List<SpecialFloor>() {
                    new SpecialFloor() { Name = "A10", Alias = "PH" },
                    new SpecialFloor() { Name = "B10", Alias = "PH" },
                    new SpecialFloor() { Name = "C12", Alias = "PH" },
                    new SpecialFloor() { Name = "D12", Alias = "PH" },
                };

                var gyms = new List<Gym>() {
                    new Gym() { Name = "A", IsOpen = true},
                    new Gym() { Name = "B", IsOpen = true}
                };

                context.Towers.AddRange(towers);
                context.SpecialFloors.AddRange(specialFloors);
                context.Gyms.AddRange(gyms);

                var rules = new Rules() {
                    Version = 0,
                    PersonsPerTimeSlot = 4,
                    HabitantsPerApartment = 4,
                    DoorsPerFloor = 8,
                    TimeSlotSpan = DefaultTimeSlotSpan,
                    Towers = towers,
                    SpecialFloors = specialFloors,
                    BannedApartments = { },
                    Gyms = gyms
                };

                context.Rules.Add(rules);
                context.SaveChanges();
            }

            if (context.TimeSlots.Any()) {
                context.TimeSlots.RemoveRange(context.TimeSlots);
                context.SaveChanges();
            }

            /*
            var newSlots = TimeSlotsFactory.CreateTodayTimeSlots(context, 5);
            newSlots.Wait();
            context.TimeSlots.AddRange(newSlots.Result);
            */
            context.SaveChanges();

            DummyDataFactory.CreateDummyData(context);
        }

        
    }
}
