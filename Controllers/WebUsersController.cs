using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;
using uul_api.Security;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class WebUsersController : ControllerBase {
        private readonly UULContext _context;
        
        public WebUsersController(UULContext context) {
            _context = context;
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<ICollection<UserWebInfoDTO>>> GetUsers() {
            var userInfo = SecHelper.GetUserInfo(HttpContext.User.Claims);
            try {
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).SingleOrDefaultAsync();
                if (!SecHelper.IsAdmin(user)) { // TODO move to claims
                    throw new Exception("Not admin");
                }
                var userDTOs = await _context.Users.Where(u => !u.Login.Equals(userInfo.Login) && !u.ApartmentCode.Equals(userInfo.ApartmentCode)).OrderBy(u => u.ApartmentCode).Select(u => new UserWebInfoDTO(u)).ToListAsync();
                return new OkObjectResult(userDTOs);
            } catch {
                return new ForbidResult();
            }
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<ActionResult<ICollection<UserWebInfoDTO>>> UpdateUser(UserWebInfoDTO userWebInfoDTO) {
            var userInfo = SecHelper.GetUserInfo(HttpContext.User.Claims);
            try {
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).SingleOrDefaultAsync();
                if (!SecHelper.IsAdmin(user)) { // TODO move to claims
                    throw new Exception("Not admin");
                }
                var userToUpdate = await _context.Users.FindAsync(userWebInfoDTO.ID);
                if (userToUpdate == null) {
                    return new NotFoundResult();
                }
                userToUpdate.IsActivated = userWebInfoDTO.IsActivated; // currently only this
                _context.Users.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return new OkObjectResult(userWebInfoDTO);
            } catch {
                return new ForbidResult();
            }
        }
    }
}
