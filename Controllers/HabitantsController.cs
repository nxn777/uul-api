﻿using Microsoft.AspNetCore.Authorization;
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
    public class HabitantsController : ControllerBase {
        private readonly UULContext _context;
        public HabitantsController(UULContext context) {
            _context = context;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> AddHabitant(HabitantDTO habitantDTO) { // TODO check rules before adding
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).SingleOrDefaultAsync();
                if (user == null) {
                    return Error.ProfileLookupFailed.createErrorResponse();
                }
                var habitant = new Habitant(habitantDTO) { User = user };
                _context.Habitants.Add(habitant);
                await _context.SaveChangesAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "Habitant was added", Data = userInfo };
            } catch (Exception e) {
                response = Error.EntitySavingFailed.createErrorResponse();
            }
            return response;
        }

        [HttpPost("edit")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> EditHabitant(HabitantDTO habitantDTO) {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).SingleOrDefaultAsync();
                if (user == null) {
                    return Error.ProfileLookupFailed.createErrorResponse();
                }
                var habitant = await _context.Habitants.FindAsync(habitantDTO.ID);
                if (habitant == null) {
                    return Error.HabitantLookupFailed.createErrorResponse();
                }
                habitant.Name = habitantDTO.Name;
                habitant.AvatarSrc = habitantDTO.AvatarSrc;
                _context.Habitants.Update(habitant);
                await _context.SaveChangesAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "Habitant was updated", Data = userInfo };
            } catch (Exception e) {
                response = Error.EntitySavingFailed.createErrorResponse();
            }
            return response;
        }


        [HttpPost("delete")]
        [Authorize]
        public async Task<ActionResult<UULResponse>> DeleteHabitant(HabitantDTO habitantDTO) {
            var currentUser = HttpContext.User;
            UULResponse response;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).FirstAsync();
                var existentHabitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                if (existentHabitants.Count <= 1) {
                    return Error.LastHabitantDeletion.createErrorResponse();
                }
                var habitant = await _context.Habitants.FindAsync(habitantDTO.ID);
                _context.Habitants.Remove(habitant);
                await _context.SaveChangesAsync();
                var habitants = await _context.Habitants.Where(h => h.User.ID == user.ID).Select(h => new HabitantDTO(h)).ToListAsync();
                userInfo.IsActivated = user.IsActivated;
                userInfo.Habitants = habitants;
                response = new UULResponse() { Success = true, Message = "Habitant was deleted", Data = userInfo };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }
    }
}
