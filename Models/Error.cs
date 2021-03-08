using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public enum Error {
        // Db 
        EntitySavingFailed = 601,
        EntityDeletionFailed = 602,
        EntityRetrievingFailed = 603,
        // Profile errors
        ProfileLookupFailed = 1001,
        ProfileHabitantLookupFailed = 1002,
        ProfileLastHabitantDeletion = 1003,
        ProfileNotActivated = 1004,
        // Rules errors
        RulesNotFound = 2001,
        // TimeSlot errors
        TimeSlotNotFound = 3001,
        TimeSlotNotToday = 3002,
        TimeSlotFull = 3003,
        TimeSlotOverbooking = 3004,
        // Gym errors
        GymClosed = 4001,
    }

    public static class ErrorDescription {
        public static string Desc(this Error error) {
            switch (error) {
                case Error.ProfileLookupFailed:
                    return "Profile not found";
                case Error.EntitySavingFailed:
                    return "Failed to save objects to the DB";
                case Error.ProfileHabitantLookupFailed:
                    return "Inhabitant not found";
                case Error.ProfileLastHabitantDeletion:
                    return "Can not delete the last inhabitant";
                case Error.EntityDeletionFailed:
                    return "Failed to delete object(s) from the DB";
                case Error.EntityRetrievingFailed:
                    return "Failed to get object(s) from the DB";
                case Error.RulesNotFound:
                    return "Rules not found";
                case Error.ProfileNotActivated:
                    return "Profile is not activated";
                case Error.TimeSlotNotFound:
                    return "Timeslot not found";
                case Error.GymClosed:
                    return "Gym is closed";
                case Error.TimeSlotNotToday:
                    return "Only todays' timeslots can be booked";
                case Error.TimeSlotFull:
                    return "TimeSlot is full";
                case Error.TimeSlotOverbooking:
                    return "This inhabitant already booked a gym today";
            }
            return "No descriiption";
        }

        public static UULResponse CreateErrorResponse(this Error error, ILogger<object> logger, string tag, Exception ex = null) {
            logger.LogInformation(tag + ":" + (int)error + " : " + error.Desc() + "\n" + (ex?.Message ?? ""));
            return new UULResponse() { Success = false, Data = null, Code = (int)error, Message = error.Desc() };
        }
    }
}
