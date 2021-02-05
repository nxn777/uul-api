using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class User {
        public long ID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActivated { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string ApartmentCode { get; set; }
        public ICollection<Habitant> Habitants { get; set; }
    }

    public class NewUserDTO {
        public string ApartmentCode { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }
        public string AvatarSrc { get; set; }
    }

    public class UserInfoDTO {
        public string ApartmentCode { get; set; }
        public string Name { get; set; }
        public bool IsActivated { get; set; }
        public IEnumerable<HabitantDTO> Habitants { get; set; }
        public UserInfoDTO() { }
        public UserInfoDTO(User user) {
            this.ApartmentCode = user.ApartmentCode;
            this.Name = user.Name;
            this.IsActivated = user.IsActivated;
            this.Habitants = user.Habitants.Select(h => new HabitantDTO(h)).ToList();
        }
    }

    public class UserLoginInfoDTO {
        public string ApartmentCode { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }
    }

    public class UserUpdateDTO {
        public long AppartmentID { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string NewHash { get; set; }
    }
}
