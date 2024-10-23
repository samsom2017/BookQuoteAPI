using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookQuoteAPI.Data
{
    public class AuthDbContext(DbContextOptions options) : IdentityDbContext<IdentityUser>(options)
    {
    }
}
