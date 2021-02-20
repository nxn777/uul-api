using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class Tower {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
        public int FloorsCount { get; set; }
    }

    public class TowerDTO {
        public string Name { get; set; }
        public int FloorsCount { get; set; }

        public TowerDTO(Tower tower) {
            Name = tower.Name;
            FloorsCount = tower.FloorsCount;
        }
    }

    public class SpecialFloor {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class SpecialFloorDTO {
        public string Name { get; set; }
        public string Alias { get; set; }
        public SpecialFloorDTO(SpecialFloor specialFloor) {
            Name = specialFloor.Name;
            Alias = specialFloor.Alias;
        }
    }

    public class BannedApartment {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
    }

    public class BannedApartmentDTO {
        public string Name { get; set; }
        public BannedApartmentDTO(BannedApartment bannedApartment) {
            Name = bannedApartment.Name;
        }
    }

    public class Gym {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }
    }

    public class GymDTO {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public GymDTO(Gym gym) {
            Name = gym.Name;
            IsOpen = gym.IsOpen;
        }
    }
}
