﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public enum Error {
        // Auth
        AuthFailed = 101,
        // Profile errors
        ProfileNotFound = 1001,
        ProfileHabitantLookupFailed = 1002,
        ProfileLastHabitantDeletion = 1003,
        ProfileNotActivated = 1004,
        ProfileValidationFailed = 1005,
        ProfileChangePwdFailed = 1006,
        ProfileAddHabitantFailed = 1007,
        ProfileEditHabitantFailed = 1008,
        ProfileDeleteHabitantFailed = 1009,
        ProfileDeletionFailed = 1010,
        ProfileLoginFailed = 1011,
        ProfileAlreadyExists = 1012,
        ProfileCreationFailed = 1013,
        ProfileGetInfoFailed = 1014,
        // Rules errors
        RulesNotFound = 2001,
        RulesGetFailed = 2002,
        // TimeSlot errors
        TimeSlotNotFound = 3001,
        TimeSlotNotToday = 3002,
        TimeSlotFull = 3003,
        TimeSlotOverbooking = 3004,
        TimeSlotsBookingFailed = 3005,
        TimeSlotsGetFailed = 3006,
        // Gym errors
        GymClosed = 4001,
        // News errors
        NewsGetFailed = 5001,
    }

    public static class ErrorDescription {
        public static string Desc(this Error error) {
            switch (error) {
                case Error.ProfileNotFound:
                    return "Profile not found";
                case Error.ProfileHabitantLookupFailed:
                    return "Inhabitant not found";
                case Error.ProfileLastHabitantDeletion:
                    return "Can not delete the last inhabitant";
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
                case Error.AuthFailed:
                    return "Wrong credentials";
                case Error.ProfileValidationFailed:
                    return "Profile data is not valid";
                case Error.ProfileChangePwdFailed:
                    return "Failed to change password";
                case Error.ProfileAddHabitantFailed:
                    return "Failed to add the inhabitant";
                case Error.ProfileEditHabitantFailed:
                    return "Failed to edit the inhabitant";
                case Error.ProfileDeleteHabitantFailed:
                    return "Failed to delete the inhabitant";
                case Error.ProfileDeletionFailed:
                    return "Failed to delete the profile";
                case Error.ProfileLoginFailed:
                    return "Login failed";
                case Error.ProfileAlreadyExists:
                    return "Profile already exists";
                case Error.ProfileCreationFailed:
                    return "Profile creation failed";
                case Error.ProfileGetInfoFailed:
                    return "Profile info retrieval failed";
                case Error.RulesGetFailed:
                    return "Rules retrieval failed";
                case Error.TimeSlotsBookingFailed:
                    return "Timeslot booking failed";
                case Error.TimeSlotsGetFailed:
                    return "Timeslots retrieval failed";
                case Error.NewsGetFailed:
                    return "News retrieval failed";
            }
            return "No description";
        }

        public static UULResponse CreateErrorResponse(this Error error, ILogger<object> logger, string tag, Exception ex = null) {
            logger.LogInformation(tag + ":" + (int)error + " : " + error.Desc() + "\n" + (ex?.Message ?? ""));
            return new UULResponse() { Success = false, Data = null, Code = (int)error, Message = error.Desc() };
        }
    }

    public class AuthException : Exception {
        public AuthException(string message) : base(message) {
        }
    }

    public class UserProfileNotFoundException : Exception {
        public UserProfileNotFoundException() {
        }

        public UserProfileNotFoundException(string message) : base(message) { }
    }
}
