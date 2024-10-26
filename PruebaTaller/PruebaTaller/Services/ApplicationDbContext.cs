using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;

namespace PruebaTaller.Services
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
           

    }

    }
}


