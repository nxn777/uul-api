using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class User {
        public long ID { get; set; }
        public long AppartmentID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Hash { get; set; }
    }

    public class UserDTO {
        public long AppartmentID { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
    }

    public class UserInfoDTO {
        public long AppartmentID { get; set; }
        public string Name { get; set; }
    }

    public class UserUpdateDTO {
        public long AppartmentID { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string NewHash { get; set; }
    }
}
