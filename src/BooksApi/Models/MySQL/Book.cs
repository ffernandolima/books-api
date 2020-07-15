
namespace BooksApi.Models.MySQL
{
    public class Book
    {
        public int Id { get; set; }
        public BookInfo BookInfo { get; set; }

        public Book()
        { }
    }
}
