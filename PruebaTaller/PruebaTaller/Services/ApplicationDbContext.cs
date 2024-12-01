using GEJ_Lab.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;

namespace PruebaTaller.Services
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart_Item> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<ShippingDetails> ShippingDetails { get; set; }


        // No necesitamos que PaymentData sea persistido en la base de datos,
        // así que lo marcamos como una entidad sin clave.
        public DbSet<PaymentData> PaymentData { get; set; }
        public DbSet<PayPalSettings> PayPalSettings { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura PaymentData como una entidad sin clave (no persistida en la base de datos)
            modelBuilder.Entity<PaymentData>().HasNoKey();
            modelBuilder.Entity<PayPalSettings>().HasNoKey();


            // Configuraciones adicionales si es necesario
            base.OnModelCreating(modelBuilder);
        }
    }
}
