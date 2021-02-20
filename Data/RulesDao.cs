using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class RulesDao {
        //TODO use include
        public static async Task<RulesDTO> GetCurrentRulesDTO(UULContext _context) {
            var rules = await _context.Rules.FirstAsync();
            var towers = await _context.Towers.Where(t => t.Rules.ID == rules.ID).Select(t => new TowerDTO(t)).ToListAsync();
            var specialFloors = await _context.SpecialFloors.Where(s => s.Rules.ID == rules.ID).Select(s => new SpecialFloorDTO(s)).ToListAsync();
            var bannedApartments = await _context.BannedApartments.Where(b => b.Rules.ID == rules.ID).Select(b => new BannedApartmentDTO(b)).ToListAsync();
            var rulesDto = new RulesDTO() {
                Version = rules.Version,
                PersonsPerTimeSlot = rules.PersonsPerTimeSlot,
                HabitantsPerApartment = rules.HabitantsPerApartment,
                DoorsPerFloor = rules.DoorsPerFloor,
                TimeSlotSpan = rules.TimeSlotSpan,
                Towers = towers,
                BannedApartments = bannedApartments,
                SpecialFloors = specialFloors
            };
            return rulesDto;
        }
    }
}
