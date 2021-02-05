using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class Rules {
        public long ID { get; set; }
        public int PersonsPerTimeSlot { get; set; }
        public int HabitantsPerApartment { get; set; }
        public int DoorsPerFloor { get; set; }
        public ICollection<Tower> Towers { get; set; }
        public ICollection<SpecialFloor> SpecialFloors { get; set; }
        public ICollection<BannedApartment> BannedApartments { get; set; }
    }
}
