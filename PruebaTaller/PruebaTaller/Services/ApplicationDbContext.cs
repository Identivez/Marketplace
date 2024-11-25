using GEJ_Lab.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;

namespace PruebaTaller.Services
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart_Item> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }   


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    var connectionString = "Server=.;Database=Prueba;User Id=myUser;Password=myPassword;"; // Reemplaza por tu cadena de conexión real
                    optionsBuilder.UseSqlServer(connectionString, options =>
                        options.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null));
                }
            }
        }
    }

    

