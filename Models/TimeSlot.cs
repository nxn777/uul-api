using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class TimeSlot {
        public long ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ICollection<User> OccupiedBy { get; set; }
    }
}
