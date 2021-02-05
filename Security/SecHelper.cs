﻿using Microsoft.Extensions.Configuration;
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

namespace uul_api.Security {

    public class SecHelper {
        private static readonly string ClaimName = "Name";
        private static readonly string ClaimApartmentCode = "ApartmentCode";
        public static string GenerateJSONWebToken(UserInfoDTO userInfo, IConfiguration _config) {
            var key = _config["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimName, userInfo.Name),
                new Claim(ClaimApartmentCode, userInfo.ApartmentCode)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(360),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static UserInfoDTO GetUserInfo(IEnumerable<Claim> claims) {
            var name = claims.Where(e => e.Type.Equals(ClaimName)).First();
            var apartmentCode = claims.Where(e => e.Type.Equals(ClaimApartmentCode)).First();
            return new UserInfoDTO() { Name = name.Value, ApartmentCode = apartmentCode.Value };
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
    }


}