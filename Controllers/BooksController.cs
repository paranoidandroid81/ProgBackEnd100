using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDataContext _libContext;

        public BooksController(LibraryDataContext libContext)
        {
            _libContext = libContext;
        }

        [HttpGet("/books/{id:int}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var resp = await _libContext.Books
                .Where(b => b.Id == id)
                .Select(b => new GetBookDetailsResponse
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    NumberOfPages = b.NumberOfPages
                }).SingleOrDefaultAsync();

            if (resp is null)
            {
                return NotFound("No book with that Id!");
            }
            return Ok(resp);
        }

        [HttpGet("/books")]
        public async Task<IActionResult> GetAllBooks([FromQuery] string genre = "all")
        {
            var books = await _libContext.Books
                .Where(b => (genre == "all") ? b.InInventory : b.InInventory && b.Genre == genre)
                .Select(b => new BookSummaryItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre
                }).ToListAsync();
            return Ok(new GetBooksResponse 
            { 
                Data = books, 
                Genre = genre,
                Count = books.Count()
            });
        }

    }
}
