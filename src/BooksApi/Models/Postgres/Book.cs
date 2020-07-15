
namespace BooksApi.Models.Postgres
{
    public class Book
    {
        public int Id { get; set; }
        public BookInfo BookInfo { get; set; }

        public Book() 
        { }
    }
}
