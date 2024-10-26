using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para el proceso de inicio de sesión.
    /// Contiene los datos necesarios para autenticar a un usuario en el sistema.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Obtiene o establece el correo electrónico del usuario.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Email { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la contraseña del usuario.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Password { get; set; } = "";

        /// <summary>
        /// Indica si el usuario desea que su sesión se recuerde en el navegador.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
