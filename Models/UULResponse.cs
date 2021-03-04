using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class UULResponse {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class UULResponse<T> {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
