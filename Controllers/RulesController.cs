using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using uul_api.Data;
using uul_api.Models;

namespace uul_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase {
        private readonly UULContext _context;
        private readonly ILogger<RulesController> _logger;
        public RulesController(UULContext context, ILogger<RulesController> logger) {
            _context = context;
            _logger = logger;
        }

        // GET: api/Rules
        [HttpGet]
        public async Task<ActionResult<UULResponse>> GetRules() {
            UULResponse response;
            try {
                var rulesDTO = await RulesDao.GetCurrentRulesDTOOrDefault(_context);
                response = new UULResponse() { Success = true, Message = "Active Rules", Data = rulesDTO };
            } catch (Exception e) {
                response = Error.RulesGetFailed.CreateErrorResponse(_logger, "GetRules", e);
            }
            return response;
        }

    }
}
