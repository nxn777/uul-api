using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class Rules {
        public long ID { get; set; }
        public int Version { get; set; }
        public int PersonsPerTimeSlot { get; set; }
        public int HabitantsPerApartment { get; set; }
        public int DoorsPerFloor { get; set; }
        public int TimeSlotSpan { get; set; }
        public ICollection<Tower> Towers { get; set; }
        public ICollection<SpecialFloor> SpecialFloors { get; set; }
        public ICollection<BannedApartment> BannedApartments { get; set; }
        public ICollection<Gym> Gyms { get; set; }
    }

    public class RulesDTO {
        public int Version { get; set; }
        public int PersonsPerTimeSlot { get; set; }
        public int HabitantsPerApartment { get; set; }
        public int DoorsPerFloor { get; set; }
        public int TimeSlotSpan { get; set; }
        public ICollection<TowerDTO> Towers { get; set; }
        public ICollection<SpecialFloorDTO> SpecialFloors { get; set; }
        public ICollection<BannedApartmentDTO> BannedApartments { get; set; }
        public ICollection<GymDTO> Gyms { get; set; }

        public RulesDTO(Rules rules) {
            Version = rules.Version;
            PersonsPerTimeSlot = rules.PersonsPerTimeSlot;
            HabitantsPerApartment = rules.HabitantsPerApartment;
            DoorsPerFloor = rules.DoorsPerFloor;
            TimeSlotSpan = rules.TimeSlotSpan;
            Towers = rules.Towers.Select(t => new TowerDTO(t)).ToList();
            BannedApartments = rules.BannedApartments.Select(b => new BannedApartmentDTO(b)).ToList();
            SpecialFloors = rules.SpecialFloors.Select(s => new SpecialFloorDTO(s)).ToList();
            Gyms = rules.Gyms.Select(g => new GymDTO(g)).ToList();
        }
    }
}
