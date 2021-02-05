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

    public class SpecialFloor {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class BannedApartment {
        public long ID { get; set; }
        public Rules Rules { get; set; }
        public string Name { get; set; }
    }
}
