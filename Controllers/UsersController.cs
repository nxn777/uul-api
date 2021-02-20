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
using uul_api.Data;
using uul_api.Models;
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

        [HttpPost("changepwd")]
        public async Task<ActionResult<UULResponse>> ChangePassword(UserUpdatePasswordDTO userPwdsDTO) {
            if (!userPwdsDTO.isValid(out var msg)) {
                return new UULResponse() { Success = false, Message = msg, Data = null };
            }
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUser(userPwdsDTO.toLoginInfoDTO());
                var user = await _context.Users.Where(u => u.Login.Equals(userInfoDTO.Login) && u.ApartmentCode.Equals(userInfoDTO.ApartmentCode)).FirstAsync();
                var salt = SecHelper.CreateSalt();
                user.Salt = salt;
                user.Hash = SecHelper.SaltAndHashPwd(userPwdsDTO.NewPwd, salt);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var tokenString = SecHelper.GenerateJSONWebToken(userInfoDTO.Login, userInfoDTO.ApartmentCode, _config);
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                response = new UULResponse() { Success = true, Message = tokenString, Data = new UserInfoDTO(user, habitants) };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpPost("delete")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> DeleteUser(UserLoginInfoDTO loginInfoDTO) {
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUser(loginInfoDTO);
                var user = await UserDao.GetUserByDetails(_context, userInfoDTO.Login, userInfoDTO.ApartmentCode);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                response = new UULResponse() { Success = true, Message = "Profile was deleted", Data = null };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UULResponse>> LoginUser(UserLoginInfoDTO loginInfoDTO) {
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUser(loginInfoDTO);
                var tokenString = SecHelper.GenerateJSONWebToken(userInfoDTO.Login, userInfoDTO.ApartmentCode, _config);
                response = new UULResponse() { Success = true, Message = "Login success", Data = tokenString };
            } catch {
                response = new UULResponse() { Success = false, Message = "Login failed", Data = null };
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public async Task<ActionResult<UULResponse>> NewUser(NewUserDTO newUser) {
            if (!newUser.isValid(out var msg)) {
                return new UULResponse() { Success = false, Message = msg, Data = null };
            }
            var exist = await _context.Users.AnyAsync(u => u.Login.Equals(newUser.Login) && u.ApartmentCode == newUser.ApartmentCode);

            if (exist) {
                return new UULResponse() { Success = false, Message = "User already exists", Data = null };
            }

            var user = UserDao.AddFromDto(_context, newUser);
            await _context.SaveChangesAsync();
            var userInfo = new UserInfoDTO(user);
            var tokenString = SecHelper.GenerateJSONWebToken(userInfo.Login, userInfo.ApartmentCode, _config);

            return new UULResponse() { Success = true, Message = tokenString, Data = userInfo };
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> GetMyUserInfo() {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).FirstAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "", Data = userInfo };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }


        private async Task<UserInfoDTO> AuthenticateUser(UserLoginInfoDTO loginInfoDTO) {
            var stored = await UserDao.GetUserByDetails(_context, loginInfoDTO.Login, loginInfoDTO.ApartmentCode);
            var saltedAndHashedPwd = SecHelper.SaltAndHashPwd(loginInfoDTO.Pwd, stored.Salt);
            if (saltedAndHashedPwd != stored.Hash) {
                throw new ArgumentException("Wrong credentials");
            }
            return new UserInfoDTO() { ApartmentCode = loginInfoDTO.ApartmentCode, Login = loginInfoDTO.Login };
        }

    }
}
