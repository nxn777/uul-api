using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {
    public class News {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string DateTime { get; set; }
    }

    public class NewsDTO {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string DateTime { get; set; }

        public NewsDTO(News news) {
            Title = news.Title;
            Content = news.Content;
            Author = news.Content;
            DateTime = news.DateTime;
        }
    }

    public class NewsWebDTO {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string DateTime { get; set; }
        public NewsWebDTO(News news) {
            ID = news.ID;
            Title = news.Title;
            Content = news.Content;
            Author = news.Content;
            DateTime = news.DateTime;
        }
    }
}
}
