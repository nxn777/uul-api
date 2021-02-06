using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uul_api.Models;

namespace uul_api.Models {
    public class TimeSlot {
        public long ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ICollection<User> OccupiedBy { get; set; }

        public TimeSlotInfoDTO ToDTO() {
            return new TimeSlotInfoDTO {
                ID = this.ID,
                Start = this.Start,
                End = this.End,
                OccupiedBy = {}
            };
        }
    }

    public class TimeSlotInfoDTO {
        public long ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ICollection<UserInfoDTO> OccupiedBy { get; set; }
    }
}
