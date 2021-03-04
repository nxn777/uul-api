using Microsoft.AspNetCore.Authorization;
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
    public class WebNewsController : ControllerBase {
        private readonly UULContext _context;

        public WebNewsController(UULContext context) {
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
                response = new UULResponse() { Success = true, Message = "News list", Data = newsListDTO.Select(n => new NewsWebDTO(n)) };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UULResponse>> GetNews(int id) {
            UULResponse response;
            var currentUser = HttpContext.User;
            try {
                var auditory = Auditory.GUESTS;
                if (currentUser.Identity.IsAuthenticated) {
                    auditory = (await UserDao.GetUserFromClaimsOrThrow(_context, currentUser)).IsActivated ? Auditory.ACTIVATED : Auditory.REGISTERED;
                }
                var newsDTO = await NewsDao.GetNewsByIdAsync(_context, auditory, id);
                response = new UULResponse() { Success = true, Message = "News item", Data = new NewsWebDTO(newsDTO) };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpPost("news")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> CreateOrUpdateNews(NewsWebDTO dto) {
            UULResponse response;
            try {
                var user = await UserDao.GetUserFromClaimsOrThrow(_context, HttpContext.User);
                if (!SecHelper.IsAdmin(user)) {
                    throw new Exception("Access denied");
                }
                var news = new News(dto);
                string message = "News was created";
                if (news.ID == null) {
                    _context.News.Add(news);
                } else {
                    _context.News.Update(news);
                    message = "News was upadted";
                }
                await _context.SaveChangesAsync();
                response = new UULResponse() { Success = true, Message = message, Data = new NewsWebDTO(news) };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }
    }
}
