using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksApi.Models.MySQL
{
    [NotMapped]
    public class BookInfo
    {
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }

        public BookInfo()
        { }
    }
}
