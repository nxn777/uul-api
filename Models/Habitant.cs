using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class Habitant {
        public long ID { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string AvatarSrc { get; set; }
        public DateTime LastGymVisit { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; }
        public Habitant() { }

        public Habitant(NewUserDTO userInfo) {
            Name = userInfo.Name;
            AvatarSrc = userInfo.AvatarSrc;
        }

        public Habitant(HabitantDTO habitantDTO) {
            Name = habitantDTO.Name;
            AvatarSrc = habitantDTO.AvatarSrc;
        }
    }

    public class HabitantDTO {
        public long ID { get; set; }
        public string Name { get; set; }
        public string AvatarSrc { get; set; }
        public string ApartmentCode { get; set; }
        public DateTime LastGymVisit { get; set; }
        public HabitantDTO() { }
        public HabitantDTO(Habitant habitant) {
            ID = habitant.ID;
            Name = habitant.Name;
            AvatarSrc = habitant.AvatarSrc;
            ApartmentCode = habitant.User.ApartmentCode;
            LastGymVisit = habitant.LastGymVisit;
        }
    }
 }
