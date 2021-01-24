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

        // GET: api/TimeSlots
        [HttpGet]
        public IActionResult GetTimeSlots() {
            return NoContent();//return await _context.TimeSlots.OrderBy(ts => ts.Start).ToListAsync();
        }

        // GET: api/TimeSlots/2020/12/15
        [HttpGet("{year}/{month}/{day}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlots(int year, int month, int day) {
            DateTime dateStart;

            try {
                dateStart = new DateTime(year, month, day);
            } catch {
                dateStart = DateTime.Today;
            }
            dateStart = dateStart.ToUniversalTime();
            var dateEnd = dateStart.AddDays(1);

            return await _context.TimeSlots.Where(ts => ts.Start.CompareTo(dateStart) >= 0 && ts.End.CompareTo(dateEnd) < 0).OrderBy(ts => ts.Start).ToListAsync();
        }

        // PUT: api/TimeSlots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeSlot(long id, TimeSlot timeSlot) {
            if (id != timeSlot.ID) {
                return BadRequest();
            }

            _context.Entry(timeSlot).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TimeSlotExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TimeSlots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeSlot>> PostTimeSlot(TimeSlot timeSlot) {
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeSlot", new { id = timeSlot.ID }, timeSlot);
        }

        // DELETE: api/TimeSlots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(long id) {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null) {
                return NotFound();
            }

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeSlotExists(long id) {
            return _context.TimeSlots.Any(e => e.ID == id);
        }
    }
}
