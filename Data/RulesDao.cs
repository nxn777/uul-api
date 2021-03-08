using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Data {
    public class RulesDao {
        
        public static async Task<RulesDTO> GetCurrentRulesDTOOrDefault(UULContext _context) {
            var rules = await _context.Rules
                .AsSplitQuery()
                .Include(r => r.Towers)
                .Include(r => r.SpecialFloors)
                .Include(r => r.BannedApartments)
                .Include(r => r.Gyms)
                .OrderByDescending(r => r.Version)
                .FirstOrDefaultAsync();
            if (rules == null) { return null; }
            return new RulesDTO(rules);
        }

        public static async Task<Rules> GetCurrentRulesOrDefault(UULContext _context) {
            var rules = await _context.Rules
                .AsSplitQuery()
                .Include(r => r.Towers)
                .Include(r => r.SpecialFloors)
                .Include(r => r.BannedApartments)
                .Include(r => r.Gyms)
                .OrderByDescending(r => r.Version)
                .FirstOrDefaultAsync();
            return rules;
        }
    }
}
