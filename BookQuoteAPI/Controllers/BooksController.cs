
using BookQuoteAPI.Data;
using BookQuoteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookQuoteAPI.Controllers
{
    // launchSetting.json can be  found http://localhost:5134/swagger/index.html
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class BooksController : ControllerBase
    {
        //inject DB
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;



        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .ToListAsync();
            return books;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var book = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound("Book is not found ");
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingBook = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingBook == null)
            {
                return NotFound("Book is not found ");
            }


            existingBook.Author = book.Author;
            existingBook.Title = book.Title;
            existingBook.PublishedDate = book.PublishedDate;

            await _context.SaveChangesAsync();

            return NoContent();
            //    if (id != book.Id)
            //    {
            //        return BadRequest();
            //    }

            //    _context.Entry(book).State = EntityState.Modified;

            //    try
            //    {
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!BookExists(id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }

            //    return NoContent();
        }



        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<List<Book>>> PostBook(Book book)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var newBook = new Book
            {
                UserId = new Guid(userId),
                Id = book.Id,
                Title = book.Title,
                PublishedDate = book.PublishedDate,
                Author = book.Author,
            };
            _context.Books.Add(newBook);

            await _context.SaveChangesAsync();

            return Ok(await _context.Books.ToListAsync());
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingBook = await _context.Books
                            .Where(x => x.UserId == new Guid(userId))
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (existingBook == null)
            {
                return NotFound("Book is not found ");
            }

            _context.Books.Remove(existingBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //private bool BookExists(int id)
        //{
        //    return _context.Books.Any(e => e.Id == id);
        //}
    }
}


/*
using BookQuoteAPI.Data;
using BookQuoteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookQuoteAPI.Controllers
{
    // launchSetting.json can be  found http://localhost:5134/swagger/index.html
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    { 
        //inject DB
        private readonly ApplicationDbContext _context;

       

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Books
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest("Invalid or missing user identifier.");
            }

            var books = await _context.Books
                .Where(x => x.UserId == userGuid)
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest("Invalid or missing user identifier.");
            }

            var book = await _context.Books
                .Where(x => x.UserId == userGuid)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            return Ok(book);
        }
        /*

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .ToListAsync();
            return books;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var book = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound("Book is not found ");
            }

            return Ok(book);
        }
        */
/*

// PUT: api/Books/5
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingBook = await _context.Books
                .Where(x => x.UserId == new Guid(userId))
                .FirstOrDefaultAsync(x => x.Id == id);

            if(existingBook == null)
            {
                return NotFound("Book is not found ");
            }


            existingBook.Author = book.Author;
            existingBook.Title = book.Title;
            existingBook.PublishedDate = book.PublishedDate;

            await _context.SaveChangesAsync();

            return NoContent();
            //    if (id != book.Id)
            //    {
            //        return BadRequest();
            //    }

            //    _context.Entry(book).State = EntityState.Modified;

            //    try
            //    {
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!BookExists(id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }

            //    return NoContent();
            }



        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /* 
      [HttpPost]
      public async Task<ActionResult<List<Book>>> PostBook(Book book)
      {
          var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          var newBook = new Book
          {
              UserId = new Guid(userId),
              Id = book.Id,
              Title = book.Title,
              PublishedDate = book.PublishedDate,
              Author = book.Author,
          };
          _context.Books.Add(newBook);

          await _context.SaveChangesAsync();

          return Ok(await _context.Books.ToListAsync());
      }
 
        */
/*
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user ID");
            }

            var newBook = new Book
            {
                UserId = parsedUserId,
                Title = book.Title,
                PublishedDate = book.PublishedDate,
                Author = book.Author,
            };
            _context.Books.Add(newBook);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }


        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingBook = await _context.Books
                            .Where(x => x.UserId == new Guid(userId))
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (existingBook == null)
            {
                return NotFound("Book is not found ");
            }
            
            _context.Books.Remove(existingBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //private bool BookExists(int id)
        //{
        //    return _context.Books.Any(e => e.Id == id);
        //}
    }
}


*/
