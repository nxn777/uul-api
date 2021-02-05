using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using uul_api.Models;
using uul_api.Models.Helpers;
using uul_api.Security;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UULContext _context;
        private readonly IConfiguration _config;
        public UsersController(UULContext context, IConfiguration config) {
            _context = context;
            _config = config;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO user) {

            /*   var candidate = await _context.Users.Where(u => u.Name.Equals(user.Name) && u.Hash.Equals(user.Hash) && u.AppartmentID == user.AppartmentID).FirstAsync();
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
               }*/

            return NoContent();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser(NewUserDTO user) {

            /*
                        var candidate = await _context.Users.Where(u => u.Name.Equals(user.Name) && u.Hash.Equals(user.Hash)).FirstAsync();
                        if (candidate == null) {
                            return BadRequest();
                        }
                        _context.Users.Remove(candidate);
                        await _context.SaveChangesAsync();
            */
            return new OkResult();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UULResponse>> LoginUser(UserLoginInfoDTO loginInfoDTO) {
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUser(loginInfoDTO);
                var tokenString = SecHelper.GenerateJSONWebToken(userInfoDTO, _config);
                response = new UULResponse() { Success = true, Message = "Login success", Data = tokenString };
            } catch (Exception e) {

                response = new UULResponse() { Success = false, Message = "Login failed", Data = e.Message };
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public async Task<ActionResult<UULResponse>> PostUser(NewUserDTO newUser) {
            var exist = await _context.Users.AnyAsync(u => u.Name.Equals(newUser.Name) && u.ApartmentCode == newUser.ApartmentCode);

            if (exist) {
                return new UULResponse() { Success = false, Message = "User already exists", Data = null };
            }

            var salt = SecHelper.CreateSalt();

            var habitant = new Habitant(newUser);
            _context.Habitants.Add(habitant);

            var userToSave = new User {
                Name = newUser.Name,
                IsActivated = false,
                CreatedAt = DateTime.UtcNow,
                Hash = SecHelper.SaltAndHashPwd(newUser.Pwd, salt),
                Salt = salt,
                ApartmentCode = newUser.ApartmentCode,
                Habitants = new List<Habitant>() { habitant }
            };
            _context.Users.Add(userToSave);

            
            await _context.SaveChangesAsync();


            return new UULResponse() { Success = true, Message = "User was created", Data = new UserInfoDTO(userToSave) };
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> GetMyUserInfo() {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Name.Equals(userInfo.Name) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).FirstAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "", Data = userInfo };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpPost("habitants/add")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> AddHabitant(HabitantDTO habitantDTO) {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Name.Equals(userInfo.Name) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).FirstAsync();
                var habitant = new Habitant(habitantDTO) { User = user };
                _context.Habitants.Add(habitant);
                await _context.SaveChangesAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "Habitant was added", Data = userInfo };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        private async Task<UserInfoDTO> AuthenticateUser(UserLoginInfoDTO loginInfoDTO) {
            var stored = await _context.Users.Where(u => u.Name.Equals(loginInfoDTO.Name) && u.ApartmentCode.Equals(loginInfoDTO.ApartmentCode)).FirstAsync();
            var saltedAndHashedPwd = SecHelper.SaltAndHashPwd(loginInfoDTO.Pwd, stored.Salt);
            if (saltedAndHashedPwd != stored.Hash) {
                throw new ArgumentException("Wrong credentials");
            }
            return new UserInfoDTO() { ApartmentCode = loginInfoDTO.ApartmentCode, Name = loginInfoDTO.Name };
        }

    }
}
