using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class DBInitializer {
        public static void Initialize(UULContext context) {
            context.Database.EnsureCreated();

            if (context.Appartments.Any()) {
                // return;   // DB has been seeded
                context.Appartments.RemoveRange(context.Appartments);
                context.SaveChanges();
            }

            foreach(Appartment appartment in createAppartments("A", 10, 8)) {
                context.Appartments.Add(appartment);
            }
            foreach (Appartment appartment in createAppartments("B", 10, 8)) {
                context.Appartments.Add(appartment);
            }
            foreach (Appartment appartment in createAppartments("C", 12, 8)) {
                context.Appartments.Add(appartment);
            }
            foreach (Appartment appartment in createAppartments("D", 12, 8)) {
                context.Appartments.Add(appartment);
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

        private static IList<Appartment> createAppartments(string building, int floors, int perFloor) {
            var appartments = new List<Appartment>(floors * perFloor);
            for (int i = 1; i <= floors; i++) {
                for (int j = 1; j <= perFloor; j++) {
                    Appartment appartment = new Appartment { Code = $"{building}{i:D2}{j:D2}" };
                    appartments.Add(appartment);
                }
            }
            return appartments;
        }
    }
}
