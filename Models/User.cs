using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class User {
        public long ID { get; set; }
        public long AppartmentID { get; set; }
        //public Appartment LivesIn { get; set; }
        public string Name { get; set; }
        public DateTime createdAt { get; set; }
    }
}
