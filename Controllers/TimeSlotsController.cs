﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uul_api.Data;
using uul_api.Models;
using uul_api.Operations;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase {
        private readonly UULContext _context;

        public TimeSlotsController(UULContext context) {
            _context = context;
        }

       
        [AllowAnonymous]
        [HttpGet("{gym:alpha}/{year:int}/{month:int}/{day:int}")]
        public async Task<ActionResult<UULResponse>> GetTimeSlotsByGym(string gym, int year, int month, int day) { 
            UULResponse response;
            try {
                var rulesDto = await RulesDao.GetCurrentRulesDTO(_context);
                DateOperations.GetTimeSlotsBoundsUtc(rulesDto.TimeSlotSpan, year, month, day, out DateTime start, out DateTime end);
                var slots = await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, gym, start, end);
                response = new UULResponse() { Success = true, Message = "gym:" + gym +"@"+year + "/" + month + "/" + day, Data = slots };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [AllowAnonymous]
        [HttpGet("{year:int}/{month:int}/{day:int}")]
        public async Task<ActionResult<UULResponse>> GetTimeSlots(int year, int month, int day) {
            UULResponse response;
            try {
                var rulesDto = await RulesDao.GetCurrentRulesDTO(_context);
                DateOperations.GetTimeSlotsBoundsUtc(rulesDto.TimeSlotSpan, year, month, day, out DateTime start, out DateTime end);
                var slots = await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, start, end);
                response = new UULResponse() { Success = true, Message = year + "/" + month + "/" + day, Data = slots };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpPost("book")]
        [Authorize]
        public  Task<ActionResult<UULResponse>> BookTimeSlot(BookTimeSlotDTO dto) => BookTimeSlotByGym(dto, "");

        [HttpPost("{gym:alpha}/book")]
        [Authorize]
        public Task<ActionResult<UULResponse>> BookTimeSlot(string gym, BookTimeSlotDTO dto) => BookTimeSlotByGym(dto, gym);

        private async Task<ActionResult<UULResponse>> BookTimeSlotByGym(BookTimeSlotDTO dto, string gym) {
            UULResponse response;
            try {
                var timeSlot = await _context.TimeSlots
                    .Include(t => t.OccupiedBy)
                    .Include(t => t.Gym)
                    .FirstOrDefaultAsync(t => t.ID == dto.TimeslotId) ?? throw new Exception("Timeslot not found");
                var rulesDto = await RulesDao.GetCurrentRulesDTO(_context);

                DateOperations.GetTodayTimeSlotsBoundsUtc(rulesDto.TimeSlotSpan, out DateTime todayStart, out DateTime todayEnd);

                if (!timeSlot.Gym.IsOpen) {
                    throw new Exception("Gym " + timeSlot.Gym.Name + " is closed");
                }
                if (!(timeSlot.Start.IsWithinBounds(todayStart, todayEnd))) {
                    throw new Exception("Only current day is available");
                }
                if (timeSlot.OccupiedBy.Count >= rulesDto.PersonsPerTimeSlot) {
                    throw new Exception("The timeslot is full");
                }
                if (await AlreadyBookedInBoundsUTC(dto.HabitantId, todayStart, todayEnd)) {
                    throw new Exception("This inhabitant already booked a gym today");
                }

                Habitant habitant = await _context.Habitants.FindAsync(dto.HabitantId) ?? throw new Exception("Inhabitant not found");
                timeSlot.OccupiedBy.Add(habitant);
                habitant.LastGymVisit = timeSlot.Start;
                _context.TimeSlots.Update(timeSlot);
                _context.Habitants.Update(habitant);
                await _context.SaveChangesAsync();

                var slots = gym.Length == 0 ? await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, todayStart, todayEnd) : await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, gym, todayStart, todayEnd);
                response = new UULResponse() { Success = true, Message = "Booked", Data = slots };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        private  Task<bool> AlreadyBookedInBoundsUTC(long habitantId, DateTime dateStart, DateTime dateEnd) {          
            var alreadyBookedToday = _context.TimeSlots
                    .Where(ts => ts.Start.CompareTo(dateStart) >= 0 && ts.End.CompareTo(dateEnd) < 0)
                    .Include(ts => ts.OccupiedBy)
                    .SelectMany(ts => ts.OccupiedBy)
                    .Where(h => h.ID == habitantId)
                    .AnyAsync();
            return alreadyBookedToday;
        }
    }
}
