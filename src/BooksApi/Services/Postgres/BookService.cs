using BooksApi.Models.Postgres;
using EntityFrameworkCore.QueryBuilder.Interfaces;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Collections.Generic;

namespace BooksApi.Services.Postgres
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork<Context> _unitOfWork;

        public BookService(IUnitOfWork<Context> unitOfWork) =>
            _unitOfWork = unitOfWork;

        public List<Book> Get(string author = null)
        {
            var repository = _unitOfWork.Repository<Book>();

            // Using IQueryable:
            // var query = repository.MultipleResultQuery();

            // if (!string.IsNullOrWhiteSpace(author))
            // {
            //     query = query.AndFilter(book => book.BookInfo.Author == author) as IMultipleResultQuery<Book>;
            // }
            //
            // var books = repository.Search(query) as List<Book>;

            // Using CommandText:
            var sql = "SELECT * FROM \"Books\"";

            if (!string.IsNullOrWhiteSpace(author))
            {
                sql += $" WHERE \"BookInfo\" -> 'Author' = '\"{author}\"';";
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
