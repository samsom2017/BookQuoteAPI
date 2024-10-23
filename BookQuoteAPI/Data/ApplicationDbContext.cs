using BookQuoteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookQuoteAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //Add DB (tables)
        public DbSet<Book> Books { get; set; }
        public DbSet<Quote> Quotes { get; set; }
    }
}
