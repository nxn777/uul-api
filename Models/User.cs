using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class User {
        public long ID { get; set; }
        public string Login { get; set; }
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
        public string Login { get; set; }
        public string Pwd { get; set; }
        public string AvatarSrc { get; set; }

        public bool isValid(out string message) {
            if (ApartmentCode == null || ApartmentCode.Length == 0) {
                message = "Incorrect Apartment";
                return false;
            }
            if (Name == null || Name.Length == 0) {
                message = "Name is too short";
                return false;
            }
            if (Login == null || Login.Length == 0) {
                message = "Login is too short";
                return false;
            }
            if (Pwd == null || Pwd.Length < 5) {
                message = "Password is too short";
                return false;
            }
            message = "";
            return true;
        }

        public UserLoginInfoDTO toLoginInfoDTO() {
            return new UserLoginInfoDTO() { ApartmentCode = this.ApartmentCode, Login = this.Login, Pwd = this.Pwd };
        }
    }

    public class UserInfoDTO {
        public string ApartmentCode { get; set; }
        public string Login { get; set; }
        public bool IsActivated { get; set; }
        public IEnumerable<HabitantDTO> Habitants { get; set; }
        public UserInfoDTO() { }
        public UserInfoDTO(User user) {
            this.ApartmentCode = user.ApartmentCode;
            this.Login = user.Login;
            this.IsActivated = user.IsActivated;
            this.Habitants = user.Habitants.Select(h => new HabitantDTO(h)).ToList();
        }
    }

    public class UserLoginInfoDTO {
        public string ApartmentCode { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
    }

    public class UserUpdateDTO {
        public long AppartmentID { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string NewHash { get; set; }
    }
}
