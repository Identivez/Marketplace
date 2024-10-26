using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para transferir la información de registro de un usuario.
    /// Incluye datos como nombre, apellido, correo electrónico, número de teléfono, dirección, contraseña y confirmación de contraseña.
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// Obtiene o establece el primer nombre del usuario.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required(ErrorMessage = "The First Name field is required"), MaxLength(100)]
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el apellido del usuario.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required(ErrorMessage = "The Last Name field is required"), MaxLength(100)]
        public string LastName { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el correo electrónico del usuario.
        /// Campo requerido con formato de dirección de correo válida y longitud máxima de 100 caracteres.
        /// </summary>
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el número de teléfono del usuario.
        /// Campo opcional con formato de número de teléfono válido y longitud máxima de 20 caracteres.
        /// </summary>
        [Phone(ErrorMessage = "The format of the Phone Number is not valid"), MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección del usuario.
        /// Campo requerido con una longitud máxima de 200 caracteres.
        /// </summary>
        [Required, MaxLength(200)]
        public string Address { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la contraseña del usuario.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Password { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la confirmación de la contraseña del usuario.
        /// Campo requerido que debe coincidir con la propiedad <see cref="Password"/>.
        /// </summary>
        [Required(ErrorMessage = "The Confirm Password field is required")]
        [Compare("Password", ErrorMessage = "Confirm Password and Password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
