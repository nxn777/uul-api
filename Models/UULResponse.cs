using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class UULResponse {
        public bool Success { get; set; }
        public String Message { get; set; }
        public Object Data { get; set; }
    }
}
