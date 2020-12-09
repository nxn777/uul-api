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
    public class AppartmentsController : ControllerBase
    {
        private readonly UULContext _context;

        public AppartmentsController(UULContext context)
        {
            _context = context;
        }

        // GET: api/Appartments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appartment>>> GetAppartments()
        {
            return await _context.Appartments.ToListAsync();
        }

        // GET: api/Appartments/5
        [HttpGet("building/{code}")]
        public async Task<ActionResult<IEnumerable<Appartment>>> GetAppartments(string code)
        {
            var appartments = await _context.Appartments.Where(a => a.Code.Contains(code)).OrderBy(a => a.Code).ToListAsync();

            if (appartments == null)
            {
                return NotFound();
            }

            return appartments;
        }

        // PUT: api/Appartments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppartment(long id, Appartment appartment)
        {
            return BadRequest();
            /*
            if (id != appartment.ID)
            {
                return BadRequest();
            }

            _context.Entry(appartment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
            */
        }

        // POST: api/Appartments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appartment>> PostAppartment(Appartment appartment)
        {
            return BadRequest();
            /*
            _context.Appartments.Add(appartment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppartment", new { id = appartment.ID }, appartment);
            */
        }

        // DELETE: api/Appartments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppartment(long id)
        {
            return BadRequest();
            /*
            var appartment = await _context.Appartments.FindAsync(id);
            if (appartment == null)
            {
                return NotFound();
            }

            _context.Appartments.Remove(appartment);
            await _context.SaveChangesAsync();

            return NoContent();
            */
        }

        private bool AppartmentExists(long id)
        {
            return _context.Appartments.Any(e => e.ID == id);
        }
    }
}
