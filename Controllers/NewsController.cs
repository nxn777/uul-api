using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Data;
using uul_api.Models;
using uul_api.Security;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase {
        private readonly UULContext _context;

        public NewsController(UULContext context) {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<ActionResult<UULResponse>> GetNews() {
            UULResponse response;
            var currentUser = HttpContext.User;
            try {
                var auditory = Auditory.GUESTS;
                if (currentUser.Identity.IsAuthenticated) {
                    auditory = (await UserDao.GetUserFromClaimsOrThrow(_context, currentUser)).IsActivated ? Auditory.ACTIVATED : Auditory.REGISTERED;
                }
                var newsListDTO = await NewsDao.GetNewsAsync(_context, auditory);
                response = new UULResponse() { Success = true, Message = "Active Rules", Data = newsListDTO };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }
    }
}
