using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class DBInitializer {
        public static void Initialize(UULContext context) {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            if (!context.Rules.Any()) {
                context.Towers.RemoveRange(context.Towers);
                context.SpecialFloors.RemoveRange(context.SpecialFloors);
                context.BannedApartments.RemoveRange(context.BannedApartments);

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

                context.Towers.AddRange(towers);
                context.SpecialFloors.AddRange(specialFloors);

                var rules = new Rules() {
                    PersonsPerTimeSlot = 4,
                    HabitantsPerApartment = 4,
                    DoorsPerFloor = 8,
                    Towers = towers,
                    SpecialFloors = specialFloors,
                    BannedApartments = { }
                };

                context.Rules.Add(rules);
                context.SaveChanges();
            }

            if (context.TimeSlots.Any()) {
                context.TimeSlots.RemoveRange(context.TimeSlots);
                context.SaveChanges();
            }


            var newSlots = DbHelper.CreateTodayTimeSlots(context, TimeSpan.FromMinutes(60), 5);
            newSlots.Wait();
            context.TimeSlots.AddRange(newSlots.Result);

            context.SaveChanges();
        }

        
    }
}
