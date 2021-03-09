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
using Microsoft.Extensions.Logging;
using uul_api.Data;
using uul_api.Models;
using uul_api.Security;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UULContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<UsersController> _logger;
        public UsersController(UULContext context, IConfiguration config, ILogger<UsersController> logger) {
            _context = context;
            _config = config;
            _logger = logger;
        }

        [HttpPost("changepwd")]
        public async Task<ActionResult<UULResponse>> ChangePassword(UserUpdatePasswordDTO userPwdsDTO) {
            if (!userPwdsDTO.isValid(out var msg)) {
                return Error.ProfileValidationFailed.CreateErrorResponse(_logger, "ChangePassword", new Exception(msg));
            }
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUserOrThrow(userPwdsDTO.toLoginInfoDTO());
                var user = await UserDao.GetUserByDetailsOrThrow(_context, userInfoDTO.Login, userInfoDTO.ApartmentCode);
                var salt = SecHelper.CreateSalt();
                user.Salt = salt;
                user.Hash = SecHelper.SaltAndHashPwd(userPwdsDTO.NewPwd, salt);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var tokenString = SecHelper.GenerateJSONWebToken(userInfoDTO.Login, userInfoDTO.ApartmentCode, _config);
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                response = new UULResponse() { Success = true, Message = tokenString, Data = new UserInfoDTO(user, habitants) };
            } catch (UserProfileNotFoundException e) {
                response = Error.ProfileNotFound.CreateErrorResponse(_logger, "ChangePassword", e);
            } catch (AuthException e) {
                response = Error.AuthFailed.CreateErrorResponse(_logger, "ChangePassword", e);
            } catch (Exception e) {
                response = Error.ProfileChangePwdFailed.CreateErrorResponse(_logger, "ChangePassword", e);
            }
            return response;
        }

        [HttpPost("delete")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> DeleteUser(UserLoginInfoDTO loginInfoDTO) {
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUserOrThrow(loginInfoDTO);
                var user = await UserDao.GetUserByDetailsOrThrow(_context, userInfoDTO.Login, userInfoDTO.ApartmentCode); 
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                response = new UULResponse() { Success = true, Message = "Profile was deleted", Data = null };
            } catch (AuthException e) {
                response = Error.AuthFailed.CreateErrorResponse(_logger, "DeleteProfile", e);
            } catch (UserProfileNotFoundException e) {
                response = Error.ProfileNotFound.CreateErrorResponse(_logger, "DeleteProfile", e);
            } catch (Exception e) {
                response = Error.ProfileDeletionFailed.CreateErrorResponse(_logger, "DeleteProfile", e);
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UULResponse>> LoginUser(UserLoginInfoDTO loginInfoDTO) {
            UULResponse response;
            try {
                var userInfoDTO = await AuthenticateUserOrThrow(loginInfoDTO);
                var tokenString = SecHelper.GenerateJSONWebToken(userInfoDTO.Login, userInfoDTO.ApartmentCode, _config);
                response = new UULResponse() { Success = true, Message = "Login success", Data = tokenString };
            } catch (AuthException e) {
                response = Error.AuthFailed.CreateErrorResponse(_logger, "Login", e);
            } catch (Exception e) {
                response = Error.ProfileLoginFailed.CreateErrorResponse(_logger, "Login", e);
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public async Task<ActionResult<UULResponse>> NewUser(NewUserDTO newUser) {
            UULResponse response;
            try {
                if (!newUser.isValid(out var msg)) {
                    return Error.ProfileValidationFailed.CreateErrorResponse(_logger, "NewProfile", new Exception(msg));
                }
                var exist = await _context.Users.AnyAsync(u => u.Login.Equals(newUser.Login) && u.ApartmentCode == newUser.ApartmentCode);

                if (exist) {
                    return Error.ProfileAlreadyExists.CreateErrorResponse(_logger, "NewProfile");
                }

                var user = UserDao.AddFromDto(_context, newUser);
                await _context.SaveChangesAsync();
                var userInfo = new UserInfoDTO(user);
                var tokenString = SecHelper.GenerateJSONWebToken(userInfo.Login, userInfo.ApartmentCode, _config);

                response = new UULResponse() { Success = true, Message = tokenString, Data = userInfo };
            } catch (Exception e) {
                response = Error.ProfileCreationFailed.CreateErrorResponse(_logger, "NewProfile", e);
            }
            return response;
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> GetMyUserInfo() {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await UserDao.GetUserByDetailsOrThrow(_context, userInfo.Login, userInfo.ApartmentCode);
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "", Data = userInfo };
            } catch (UserProfileNotFoundException e) {
                response = Error.ProfileNotFound.CreateErrorResponse(_logger, "ProfileInfo", e);
            } catch (Exception e) {
                response = Error.ProfileGetInfoFailed.CreateErrorResponse(_logger, "ProfileInfo", e);
            }
            return response;
        }

        private async Task<UserInfoDTO> AuthenticateUserOrThrow(UserLoginInfoDTO loginInfoDTO) {
            var stored = await UserDao.GetUserByDetailsOrThrow(_context, loginInfoDTO.Login, loginInfoDTO.ApartmentCode);
            var saltedAndHashedPwd = SecHelper.SaltAndHashPwd(loginInfoDTO.Pwd, stored.Salt);
            if (saltedAndHashedPwd != stored.Hash) {
                throw new AuthException("Wrong credentials");
            }
            return new UserInfoDTO() { ApartmentCode = loginInfoDTO.ApartmentCode, Login = loginInfoDTO.Login };
        }

    }
}
