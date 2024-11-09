using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;
using PruebaTaller.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar HttpClient
builder.Services.AddHttpClient();

// Registrar el servicio de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de caducar la sesión
    options.Cookie.HttpOnly = true; // Aumenta la seguridad
    options.Cookie.IsEssential = true; // Habilita las sesiones para usuarios que no han aceptado cookies no esenciales
});

// Configurar la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Asegurarse de agregar autenticación
app.UseAuthorization();

// Agregar el middleware de sesión
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Línea nueva para inicializar datos en la base de datos
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
        as UserManager<ApplicationUser>;
    var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
        as RoleManager<IdentityRole>;

    await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
}


app.Run();
