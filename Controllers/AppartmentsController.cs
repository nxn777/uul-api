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
    public class AppartmentsController : ControllerBase {
        private readonly UULContext _context;

        public AppartmentsController(UULContext context) {
            _context = context;
        }

        // GET: api/Appartments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appartment>>> GetAppartments() {
            return await _context.Appartments.ToListAsync();
        }

        // GET: api/Appartments/5
        [HttpGet("building/{code}")]
        public async Task<ActionResult<IEnumerable<Appartment>>> GetAppartments(string code) {
            var appartments = await _context.Appartments.Where(a => a.Code.Contains(code)).OrderBy(a => a.Code).ToListAsync();

            if (appartments == null) {
                return NotFound();
            }

            return appartments;
        }

        // PUT: api/Appartments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutAppartment(long id, Appartment appartment) {
            return BadRequest();
        }

        // POST: api/Appartments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("my")]
        public async Task<ActionResult<IEnumerable<UserInfoDTO>>> GetMyAppartmentInfo(UserDTO user) {
            var candidate = await _context.Users.Where(u => u.Name.Equals(user.Name) && u.Hash.Equals(user.Hash) && u.AppartmentID == user.AppartmentID).FirstAsync();
            if (candidate == null || !AppartmentExists(user.AppartmentID)) {
                return BadRequest();
            }

            return await _context.Users
                .Where(u => u.AppartmentID == user.AppartmentID)
                .Select(i => new UserInfoDTO { AppartmentID = i.AppartmentID, Name = i.Name })
                .ToListAsync();
        }

        // DELETE: api/Appartments/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAppartment(long id) {
            return BadRequest();
        }

        private bool AppartmentExists(long id) {
            return _context.Appartments.Any(e => e.ID == id);
        }
    }
}
