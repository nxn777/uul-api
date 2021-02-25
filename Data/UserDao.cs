﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;
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
                CreatedAt = DateTime.UtcNow,
                Hash = SecHelper.SaltAndHashPwd(newUser.Pwd, salt),
                Salt = salt,
                ApartmentCode = newUser.ApartmentCode,
                Habitants = new List<Habitant>() { habitant }
            };
            context.Users.Add(userToSave);

            return userToSave;
        }

        public static async Task<User> GetUserByDetails(UULContext context, string login, string apartment) {
            var user = await context.Users.Where(u => u.Login.Equals(login) && u.ApartmentCode.Equals(apartment)).FirstOrDefaultAsync();
            return user ?? throw new Exception("Wrong credentials");
        }
    }

   
}