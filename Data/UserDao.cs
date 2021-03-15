using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using uul_api.Models;
using uul_api.Operations;
using uul_api.Security;

namespace uul_api.Data {
    public static class UserDao {
        public static User AddFromDto(UULContext context, NewUserDTO newUser) {
            var salt = SecHelper.CreateSalt();

            var habitant = new Habitant(newUser);
            context.Habitants.Add(habitant);

            var userToSave = new User {
                Login = newUser.Login,
                IsActivated = false,
                CreatedAt = DateOperations.Now(),
                Hash = SecHelper.SaltAndHashPwd(newUser.Pwd, salt),
                Salt = salt,
                ApartmentCode = newUser.ApartmentCode,
                Habitants = new List<Habitant>() { habitant }
            };
            context.Users.Add(userToSave);

            return userToSave;
        }

        public static async Task<User> GetUserByDetailsOrThrow(UULContext context, string login, string apartment) {
            return (await context.Users.Where(u => u.Login.Equals(login) && u.ApartmentCode.Equals(apartment)).SingleOrDefaultAsync()) ?? throw new UserProfileNotFoundException();
        }

        public static async Task<User> GetUserFromClaimsOrThrow(UULContext _context, ClaimsPrincipal currentUser) {
            return await GetUserFromClaimsOrDefault(_context, currentUser) ?? throw new Exception("User not found");
        }

        public static async Task<User> GetUserFromClaimsOrDefault(UULContext _context, ClaimsPrincipal currentUser) {
            var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
            var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).SingleOrDefaultAsync();
            return user;
        }
    }

   
}
