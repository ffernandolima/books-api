using BooksApi.Models.Postgres;
using BooksApi.Services.Postgres;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BooksApi.Controllers.Postgres
{
    [Route("api/postgres/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService) =>
            _bookService = bookService;

        [HttpGet]
        public ActionResult<List<Book>> Get(string author = null) =>
            _bookService.Get(author);
    }
}
