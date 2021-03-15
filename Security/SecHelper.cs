using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using uul_api.Models;
using uul_api.Operations;

namespace uul_api.Security {

    public class SecHelper {
        private static readonly string Admin = "mmd900";
        private static readonly string AdminAppCode = "0000";

        private static readonly string ClaimLogin = "Login";
        private static readonly string ClaimApartmentCode = "ApartmentCode";
        public static string GenerateJSONWebToken(string login, string apartmentCode, IConfiguration _config) {
            var key = _config["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimLogin, login),
                new Claim(ClaimApartmentCode, apartmentCode)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateOperations.Now().AddDays(360),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static UserInfoDTO GetUserInfo(IEnumerable<Claim> claims) {
            var login = claims.Where(e => e.Type.Equals(ClaimLogin)).First();
            var apartmentCode = claims.Where(e => e.Type.Equals(ClaimApartmentCode)).First();
            return new UserInfoDTO() { Login = login.Value, ApartmentCode = apartmentCode.Value };
        }

        public static string SaltAndHashPwd(string pwd, string salt) {
            var sha = SHA256.Create();
            var saltedPwd = pwd + salt;
            return Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPwd)));
        }

        public static string CreateSalt() {
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static User CreateDefaultAdmin() {
            var salt = CreateSalt();
            return new User() {
                ApartmentCode = AdminAppCode,
                Login = Admin,
                IsActivated = true,
                CreatedAt = DateOperations.Now(),
                Hash = SaltAndHashPwd("thecownamedlolasayshola", salt),
                Salt = salt,
            };
        }

        public static bool IsAdmin(User user) {
            return user != null &&  user.ApartmentCode.Equals(AdminAppCode) && user.Login.Equals(Admin);
        }
    }


}
