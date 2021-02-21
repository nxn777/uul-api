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
        public ICollection<Habitant> OccupiedBy { get; set; }
        public Gym Gym { get; set; }

        public TimeSlotDTO ToDTO() {
            return new TimeSlotDTO() {
                ID = this.ID,
                Start = this.Start,
                End = this.End,
                OccupiedBy = OccupiedBy.Select(e => new HabitantDTO(e)).ToList(),
                Gym = this.Gym.Name
            };
        }
    }

    public class TimeSlotDTO {
        public long ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ICollection<HabitantDTO> OccupiedBy { get; set; }
        public string Gym { get; set; }
    }

    public class BookTimeSlotDTO {
        public long TimeslotId { get; set; }
        public long HabitantId { get; set; }
    }

    public class ScheduleDTO {
        public string Date { get; set; }
        public int? GymId { get; set; }
        public ICollection<TimeSlotDTO> TimeSlots { get; set; }
    }
}
