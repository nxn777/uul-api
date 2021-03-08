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
        HabitantLookupFailed = 1002,
        LastHabitantDeletion = 1003
    }

    public static class ErrorDescription {
        public static string Desc(this Error error) {
            switch (error) {
                case Error.ProfileLookupFailed:
                    return "Profile not found";
                case Error.EntitySavingFailed:
                    return "Failed to save objects to the DB";
                case Error.HabitantLookupFailed:
                    return "Inhabitant not found";
                case Error.LastHabitantDeletion:
                    return "Can not delete the last inhabitant";
                case Error.EntityDeletionFailed:
                    return "Failed to delete object(s) from the DB";
                case Error.EntityRetrievingFailed:
                    return "Failed to get object(s) from the DB";
            }
            return "No descriiption";
        }

        public static UULResponse createErrorResponse(this Error error) {
            return new UULResponse() { Success = false, Data = null, Code = (int)error, Message = error.Desc() };
        }
    }
}
