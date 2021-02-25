using System;
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
using uul_api.Security;

namespace uul_api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase {
        private readonly UULContext _context;

        public TimeSlotsController(UULContext context) {
            _context = context;
        }

       
        [AllowAnonymous]
        [HttpGet("{gymId:int}/{year:int}/{month:int}/{day:int}")]
        public async Task<ActionResult<UULResponse>> GetTimeSlotsByGym(int gymId, int year, int month, int day) { 
            UULResponse response;
            try {
                var rulesDto = await RulesDao.GetCurrentRulesDTO(_context);
                DateOperations.GetTimeSlotsBoundsUtc(rulesDto.TimeSlotSpan, year, month, day, out DateTime start, out DateTime end);
                var slots = await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, gymId, start, end);
                var data = new ScheduleDTO() { Date = year + "/" + month + "/" + day, GymId = gymId, TimeSlots = slots };
                response = new UULResponse() { Success = true, Message = "", Data =  data };
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
                var data = new ScheduleDTO() { Date = year + "/" + month + "/" + day, GymId = null, TimeSlots = slots };
                response = new UULResponse() { Success = true, Message = year + "/" + month + "/" + day, Data = data };
            } catch (Exception e) {
                response = new UULResponse() { Success = false, Message = e.Message, Data = null };
            }
            return response;
        }

        [HttpPost("book")]
        [Authorize]
        public  Task<ActionResult<UULResponse>> BookTimeSlot(BookTimeSlotDTO dto) => BookTimeSlotByGym(dto, -1);

        [HttpPost("{gymId:int}/book")]
        [Authorize]
        public Task<ActionResult<UULResponse>> BookTimeSlot(int gymId, BookTimeSlotDTO dto) => BookTimeSlotByGym(dto, gymId);

        private async Task<ActionResult<UULResponse>> BookTimeSlotByGym(BookTimeSlotDTO dto, int gymId) {
            UULResponse response;
            var currentUser = HttpContext.User;
            try {
                var userInfo = SecHelper.GetUserInfo(currentUser.Claims);
                var user = await _context.Users.Where(u => u.Login.Equals(userInfo.Login) && u.ApartmentCode.Equals(userInfo.ApartmentCode)).FirstAsync();
                if (!user.IsActivated) {
                    throw new Exception("User is not activated.\nPlease visit the administration.");
                }
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
                var success = await _context.SaveChangesAsync() != 0;

                var slots = gymId == -1 ? await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, todayStart, todayEnd) : await TimeSlotsDao.GetTimeSlotsByUtcBounds(_context, gymId, todayStart, todayEnd);
                var data = new ScheduleDTO() { Date = todayStart.Year + "/" + todayStart.Month + "/" + todayStart.Day, GymId = gymId == -1 ? null : gymId, TimeSlots = slots };
                response = new UULResponse() { Success = success, Message = "Booked", Data = data };
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
