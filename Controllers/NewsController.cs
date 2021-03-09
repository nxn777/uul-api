using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Data;
using uul_api.Models;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase {
        private readonly UULContext _context;
        private readonly ILogger<NewsController> _logger;
        public NewsController(UULContext context, ILogger<NewsController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<UULResponse>> GetNews() {
            UULResponse response;
            var currentUser = HttpContext.User;
            try {
                var auditory = Auditory.GUESTS;
                if (currentUser.Identity.IsAuthenticated) {
                    var user = (await UserDao.GetUserFromClaimsOrDefault(_context, currentUser));
                    if (user == null) {
                        return Error.ProfileNotFound.CreateErrorResponse(_logger, "GetNews");
                    }
                    auditory = user.IsActivated ? Auditory.ACTIVATED : Auditory.REGISTERED;
                }
                var newsListDTO = await NewsDao.GetNewsAsync(_context, auditory);
                response = new UULResponse() { Success = true, Message = "News list", Data = new NewsPaperDTO() { News = newsListDTO.Select(n => new NewsDTO(n)).ToList() } };
            } catch (Exception e) {
                response = Error.NewsGetFailed.CreateErrorResponse(_logger, "GetNews", e);
            }
            return response;
        }
    }
}
