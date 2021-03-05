using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uul_api.Models {

    public enum Auditory {
        GUESTS, REGISTERED, ACTIVATED
    }

    public class News {
        public long? ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public Auditory Auditory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public News() { }
        public News(NewsWebDTO news) {
            ID = news.ID;
            Title = news.Title;
            Content = news.Content;
            Author = news.Author;
            CreatedAt = news.CreatedAt;
            UpdatedAt = news.UpdatedAt;
            Auditory = news.Auditory;
        }
    }

    public class NewsDTO {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public NewsDTO(News news) {
            Title = news.Title;
            Content = news.Content;
            Author = news.Author;
            CreatedAt = news.CreatedAt;
            UpdatedAt = news.UpdatedAt;
            LastModifiedAt = news.CreatedAt.CompareTo(news.UpdatedAt) > 0 ? news.CreatedAt : news.UpdatedAt;
        }
    }

    public class NewsPaperDTO {
        public ICollection<NewsDTO> News { get; set; }
    }
    public class NewsWebDTO {
       
        public long? ID { get; set; }
        [Required]
        [MinLength(3)]
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        [MinLength(3)]
        public string Author { get; set; }
        public Auditory Auditory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public NewsWebDTO() { }
        public NewsWebDTO(News news) {
            ID = news.ID;
            Title = news.Title;
            Content = news.Content;
            Author = news.Author;
            CreatedAt = news.CreatedAt;
            UpdatedAt = news.UpdatedAt;
            Auditory = news.Auditory;
            LastModifiedAt = news.CreatedAt.CompareTo(news.UpdatedAt) > 0 ? news.CreatedAt : news.UpdatedAt;
        }
    }

}
