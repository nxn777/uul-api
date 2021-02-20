using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uul_api.Data;
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
                var rulesDTO = await RulesDao.GetCurrentRulesDTO(_context);
                response = new UULResponse() { Success = true, Message = "Active Rules", Data = rulesDTO };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

    }
}
