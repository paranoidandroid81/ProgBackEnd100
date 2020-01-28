using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Gives you all books currently in inventory of given context
        /// </summary>
        /// <param name="ctx">Context to search for books</param>
        /// <returns>IQueryable of books in inventory</returns>
        private IQueryable<Book> GetBooksInInventory(LibraryDataContext ctx)
        {
            return ctx.Books
                .Where(b => b.InInventory);
        }

        [HttpPut("/books/{id:int}/genre")]
        public async Task<IActionResult> UpdateTheGenre(int id, [FromBody] string newGenre)
        {
            var book = await GetBooksInInventory(_libContext).SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            book.Genre = newGenre;
            await _libContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("/books/{id:int}")]
        public async Task<IActionResult> RemoveBookFromInventory(int id)
        {
            var book = await GetBooksInInventory(_libContext)
                .SingleOrDefaultAsync(b => b.Id == id);
            if (book != null)
            {
                book.InInventory = false;
                await _libContext.SaveChangesAsync();
            }
            return NoContent();
        }

        /// <summary>
        /// Add a book to the inventory
        /// </summary>
        /// <param name="bookToAdd">Info about the book you want to add</param>
        /// <returns></returns>
        [HttpPost("/books")]
        [Produces("application/json")]
        public async Task<ActionResult<GetBookDetailsResponse>> AddABook([FromBody] PostBooksRequest bookToAdd)
        {
            // Validate it. (if invalid, return a 400 Bad Request)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Don't use imperative validation, use field validation in model class
            // Add it to the domain.
            // - PostBooksRequest -> Books
            var book = new Book
            {
                Title = bookToAdd.Title,
                Author = bookToAdd.Author,
                Genre = bookToAdd.Genre,
                NumberOfPages = bookToAdd.NumberOfPages,
                InInventory = true
            };
            // - Add it to the context
            _libContext.Books.Add(book);
            // - Have the context save everything.
            await _libContext.SaveChangesAsync(); // this also creates an ID for the book
            // Return a 201 Created Status Code.
            // - Add a location header on the response, e.g. Location: http://server/books/8
            // - Add the entity
            // - Book -> GetBooksDetailResponse

            var response = new GetBookDetailsResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                NumberOfPages = book.NumberOfPages
            };
            return CreatedAtRoute("books#getbookbyid", new {  id = response.Id }, response);
        }

        [HttpGet("/books/{id:int}", Name = "books#getbookbyid")]
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

        /// <summary>
        /// Provides a list of all the books in our inventory, optionally filtered by genre
        /// </summary>
        /// <param name="genre">Genre to filter by if desired. Otherwise, all books returned.</param>
        /// <returns>List of books found in inventory</returns>
        /// <response code="200">Returns all of your books in inventory</response>
        [HttpGet("/books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetBooksResponse>> GetAllBooks([FromQuery] string genre = "all")
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
