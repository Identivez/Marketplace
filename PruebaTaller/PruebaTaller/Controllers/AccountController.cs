using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PruebaTaller.Models;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador para gestionar el registro, inicio y cierre de sesión de los usuarios.
    /// Proporciona métodos para registrar nuevos usuarios, autenticarlos y manejar el acceso.
    /// Implementa las prácticas recomendadas de seguridad y autenticación en ASP.NET Core.
    /// </summary>
    public class AccountController : Controller
    {
        // Gestión de usuarios a través de Identity
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        /// <summary>
        /// Constructor que inicializa las dependencias de UserManager y SignInManager.
        /// </summary>
        /// <param name="userManager">Servicio de administración de usuarios.</param>
        /// <param name="signInManager">Servicio de administración de inicio de sesión.</param>
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Muestra la vista de registro de usuario.
        /// </summary>
        /// <param name="registerDTO">Datos del usuario para el registro.</param>
        /// <returns>Vista del formulario de registro.</returns>
        public IActionResult Register(RegisterDTO registerDTO)
        {
            return View();
        }

        /// <summary>
        /// Redirige al usuario a la página de inicio si el acceso ha sido denegado.
        /// </summary>
        /// <returns>Redirección a la vista "Index" del controlador "Home".</returns>
        public IActionResult AccessDenied()
        {
            return View("Index", "Home");
        }

        /// <summary>
        /// Registra un nuevo usuario de forma asíncrona.
        /// </summary>
        /// <param name="registerDTO">Datos de registro del usuario.</param>
        /// <returns>Redirige a la página principal si el registro fue exitoso, o muestra los errores de registro.</returns>
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDTO);
            }

            // Crear un nuevo usuario a partir de los datos proporcionados
            var user = new ApplicationUser()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                Address = registerDTO.Address,
                CreatedAt = DateTime.Now,
            };

            // Intentar crear el nuevo usuario con la contraseña proporcionada
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                // Si el registro es exitoso, asignar el rol de cliente y autenticar al usuario
                await userManager.AddToRoleAsync(user, "client");
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            // Mostrar los errores de registro
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDTO);
        }

        /// <summary>
        /// Cierra la sesión de usuario de forma asíncrona.
        /// </summary>
        /// <returns>Redirige a la página principal después de cerrar la sesión.</returns>
        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Muestra la vista de inicio de sesión.
        /// </summary>
        /// <returns>Vista del formulario de inicio de sesión.</returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Inicia sesión de usuario de forma asíncrona.
        /// </summary>
        /// <param name="loginDto">Datos de inicio de sesión del usuario.</param>
        /// <returns>Redirige a la página principal si el inicio de sesión fue exitoso, o muestra errores en caso de fallo.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            // Intentar iniciar sesión con las credenciales proporcionadas
            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
            }

            return View(loginDto);
        }
    }
}
