using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uul_api.Models;
using uul_api.Security;

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

        // POST: api/Appartments/my
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> GetMyAppartmentInfo() {
            var currentUser = HttpContext.User;
            UULResponse response; 
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                response = new UULResponse() { Success = true, Message = "", Data = userInfo };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }


        private bool AppartmentExists(long id) {
            return _context.Appartments.Any(e => e.ID == id);
        }
    }
}
