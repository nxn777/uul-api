using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uul_api.Models;

namespace uul_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase {
        private readonly UULContext _context;

        public RulesController(UULContext context) {
            _context = context;
        }

        // GET: api/Rules
        [HttpGet]
        public async Task<ActionResult<UULResponse>> GetRules() {
            UULResponse response;
            try {
                var rules = await _context.Rules.FirstAsync();
                var towers = await _context.Towers.Where(t => t.Rules.ID == rules.ID).Select(t => new TowerDTO(t)).ToListAsync();
                var specialFloors = await _context.SpecialFloors.Where(s => s.Rules.ID == rules.ID).Select(s => new SpecialFloorDTO(s)).ToListAsync();
                var bannedApartments = await _context.BannedApartments.Where(b => b.Rules.ID == rules.ID).Select(b => new BannedApartmentDTO(b)).ToListAsync();
                var rulesDto = new RulesDTO() {
                    Version = rules.Version,
                    PersonsPerTimeSlot = rules.PersonsPerTimeSlot,
                    HabitantsPerApartment = rules.HabitantsPerApartment,
                    DoorsPerFloor = rules.DoorsPerFloor,
                    Towers = towers,
                    BannedApartments = bannedApartments,
                    SpecialFloors = specialFloors
                };
                response = new UULResponse() { Success = true, Message = "Active Rules", Data = rulesDto };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

    }
}
