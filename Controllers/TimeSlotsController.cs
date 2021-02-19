using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uul_api.Models;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase {
        private readonly UULContext _context;

        public TimeSlotsController(UULContext context) {
            _context = context;
        }

        // GET: api/TimeSlots/2020/12/15
        [HttpGet("{year}/{month}/{day}")]
        public async Task<ActionResult<UULResponse>> GetTimeSlots(int year, int month, int day) {
            DateTime dateStart;
            UULResponse response;
            try {
                dateStart = new DateTime(year, month, day);
                dateStart = dateStart.ToUniversalTime();
                var dateEnd = dateStart.AddDays(1);
                var slots = await _context.TimeSlots
                    .Where(ts => ts.Start.CompareTo(dateStart) >= 0 && ts.End.CompareTo(dateEnd) < 0)
                    .OrderBy(ts => ts.Start)
                    .Include(slot => slot.OccupiedBy)
                    .Select(e => e.ToDTO())
                    .ToListAsync();
                response = new UULResponse() { Success = false, Message = year+"/"+month+"/"+day, Data = slots };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

    }
}
