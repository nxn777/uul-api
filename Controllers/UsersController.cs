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
    public class UsersController : ControllerBase {
        private readonly UULContext _context;

        public UsersController(UULContext context) {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetUsers() {
            return NoContent();
        }

        // GET: api/Users/5 
        [HttpGet("{id}")]
        public IActionResult GetUser(long _) {
            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO user) {

            var candidate = await _context.Users.Where(u => u.Name.Equals(user.Name) && u.Hash.Equals(user.Hash) && u.AppartmentID == user.AppartmentID).FirstAsync();
            if (candidate == null) {
                return BadRequest();
            }

            candidate.Hash = user.NewHash;

            _context.Entry(candidate).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UserExists(candidate.ID)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser(UserDTO user) {

            var candidate = await _context.Users.Where(u => u.Name.Equals(user.Name) && u.Hash.Equals(user.Hash) && u.AppartmentID == user.AppartmentID).FirstAsync();
            if (candidate == null) {
                return BadRequest();
            }
            _context.Users.Remove(candidate);
            await _context.SaveChangesAsync();
            return new OkResult();
        }

        [HttpPost("new")]
        public async Task<ActionResult<User>> PostUser(UserDTO user) {
            var userToSave = new User {
                AppartmentID = user.AppartmentID,
                Name = user.Name,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(userToSave);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = userToSave.ID }, userToSave);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(long _) {
            return NoContent();
        }

        private bool UserExists(long id) {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
