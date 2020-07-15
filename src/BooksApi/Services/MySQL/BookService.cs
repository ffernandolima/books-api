using BooksApi.Models.MySQL;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Collections.Generic;

namespace BooksApi.Services.MySQL
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork<Context> _unitOfWork;

        public BookService(IUnitOfWork<Context> unitOfWork) =>
            _unitOfWork = unitOfWork;

        public List<Book> Get(string author = null)
        {
            var repository = _unitOfWork.Repository<Book>();

            // Using CommandText:
            var sql = "SELECT * FROM `Books`";

            if (!string.IsNullOrWhiteSpace(author))
            {
                sql += $" WHERE JSON_EXTRACT(`BookInfo`, '$.Author') = '{author}';";
            }

            var books = repository.FromSql(sql) as List<Book>;

            return books;
        }
    }

    public interface IBookService
    {
        List<Book> Get(string author = null);
    }
}
